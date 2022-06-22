using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service;
using MyJetWallet.Sdk.Service.Tools;
using MyJetWallet.Sdk.ServiceBus;
using Service.AutoInvestManager.Domain.Helpers;
using Service.AutoInvestManager.Domain.Models;
using Service.EmailSender.Grpc;
using Service.EmailSender.Grpc.Models;
using Service.GroupManager.Grpc;
using Service.GroupManager.Grpc.Models;
using Service.Liquidity.Converter.Domain.Models;
using Service.Liquidity.Converter.Grpc;
using Service.Liquidity.Converter.Grpc.Models;
using Service.PersonalData.Grpc;
using Service.PersonalData.Grpc.Contracts;

namespace Service.AutoInvestManager.Worker.Jobs
{
    public class OrdersProcessingJob : IStartable
    {
        private readonly ILogger<OrdersProcessingJob> _logger;
        private readonly InstructionsRepository _repository;
        private readonly MyTaskTimer _timer;
        private readonly IQuoteService _quoteService;
        private readonly IServiceBusPublisher<InvestOrder> _publisher;
        private readonly IEmailSenderService _emailSender;
        private readonly IPersonalDataServiceGrpc _personalData;
        private readonly IGroupsService _groupsService;

        public OrdersProcessingJob(ILogger<OrdersProcessingJob> logger, InstructionsRepository repository, IQuoteService quoteService, IServiceBusPublisher<InvestOrder> publisher, IEmailSenderService emailSender, IPersonalDataServiceGrpc personalData, IGroupsService groupsService)
        {
            _logger = logger;
            _repository = repository;
            _quoteService = quoteService;
            _publisher = publisher;
            _emailSender = emailSender;
            _personalData = personalData;
            _groupsService = groupsService;
            _timer = new MyTaskTimer(typeof(OrdersProcessingJob), TimeSpan.FromSeconds(Program.Settings.TimerPeriodInSeconds), logger, DoTime);
        }

        private async Task DoTime()
        {
            await HandleActiveInstructions();
            await HandleNewOrders();
            await SendFailureEmails();
        }
        
        private async Task HandleActiveInstructions()
        {
            try
            {
                var instructions = await _repository.GetInstructionsByStatus(InstructionStatus.Active);
                foreach (var instruction in instructions)
                {
                    if(!IsExpired(instruction))
                        continue;

                    var order = new InvestOrder
                    {
                        Id = Guid.NewGuid().ToString("N"),
                        ClientId = instruction.ClientId,
                        BrokerId = instruction.BrokerId,
                        WalletId = instruction.WalletId,
                        InvestInstructionId = instruction.Id,
                        FromAmount = instruction.FromAmount,
                        FromAsset = instruction.FromAsset,
                        ToAsset = instruction.ToAsset,
                        Status = OrderStatus.Scheduled,
                        CreationTime = DateTime.UtcNow,
                        ScheduleType = instruction.ScheduleType
                    };

                    await _repository.UpsertOrders(order);
                    _logger.LogInformation("Created order {order} based on instruction {instructionId}", order.ToJson(), instruction.Id);
                    
                    instruction.LastExecutionTime = DateTime.UtcNow;
                    instruction.ShouldSendFailEmail = false;
                    instruction.ErrorText = String.Empty;
                    instruction.FailureTime = DateTime.MinValue;
                    
                    await _repository.UpsertInstructions(instruction);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When creating invest orders");
            }
        }

        private async Task HandleNewOrders()
        {
            try
            {
                var orders = await _repository.GetScheduledInvestOrders();
                if(!orders.Any())
                    return;
                
                foreach (var order in orders)
                {
                    var group = await _groupsService.GetOrCreateClientProfileWithGroup(new GetOrClientProfileRequest()
                    {
                        ClientId = order.ClientId
                    });
                    
                    var quote = await _quoteService.GetQuoteAsync(new GetQuoteRequest
                    {
                        FromAsset = order.FromAsset,
                        ToAsset = order.ToAsset,
                        FromAssetVolume = order.FromAmount,
                        ToAssetVolume = 0.0m,
                        IsFromFixed = true,
                        BrokerId = order.BrokerId,
                        AccountId = order.ClientId,
                        WalletId = order.WalletId,
                        QuoteType = QuoteType.AutoInvest,
                        RecurringBuy = null,
                        ProfileId = group.Group.ConverterProfileId
                    });
                    
                    if (!quote.IsSuccess)
                    {
                        order.Status = OrderStatus.Failed;
                        order.ErrorText = quote.ErrorMessage;
                        order.ErrorCode = GetErrorCode(quote.ErrorMessage);
                        await _repository.UpsertOrders(order);
                        await UpdateInstructionFailed(order);
                        continue;
                    }
                    
                    var quoteResult = await _quoteService.ExecuteQuoteAsync(new ExecuteQuoteRequest
                    {
                        FromAsset = quote.Data.FromAsset,
                        ToAsset = quote.Data.ToAsset,
                        FromAssetVolume = quote.Data.FromAssetVolume,
                        ToAssetVolume = quote.Data.ToAssetVolume,
                        IsFromFixed = quote.Data.IsFromFixed,
                        BrokerId = quote.Data.BrokerId,
                        AccountId = quote.Data.AccountId,
                        WalletId = quote.Data.WalletId,
                        OperationId = quote.Data.OperationId,
                        Price = quote.Data.Price,
                        RelatedRecurringBuyId = order.InvestInstructionId,
                    });
                    
                    switch (quoteResult.QuoteExecutionResult)
                    {
                        case QuoteExecutionResult.Success:
                            order.QuoteId = quoteResult.Data.OperationId;
                            order.ExecutionTime = DateTime.UtcNow;
                            order.Price = quoteResult.Data.Price;
                            order.ToAmount = quote.Data.ToAssetVolume;
                            order.FeeAmount = quote.Data.FeeAmount;
                            order.FeeCoef = quote.Data.FeeCoef;
                            order.FeeAsset = quote.Data.FeeAsset;
                            order.Status = OrderStatus.Executed;
                            await UpdateInstructionSuccess(order);
                            break;
                        case QuoteExecutionResult.Error:
                        case QuoteExecutionResult.ReQuote:
                            order.Status = OrderStatus.Failed;
                            order.ErrorText = quoteResult.ErrorMessage;
                            order.ErrorCode = GetErrorCode(quoteResult.ErrorMessage);
                            await UpdateInstructionFailed(order);
                            break;
                    }

                    await _repository.UpsertOrders(order);
                    _logger.LogInformation("Executed order {orderId} with result {quoteResul}", order.Id, quoteResult.ToJson());
                }

                await _publisher.PublishAsync(orders);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When executing invest orders");
            }
            
            //locals
            async Task UpdateInstructionFailed(InvestOrder order)
            {
                var instruction = await _repository.GetInstructionById(order.InvestInstructionId);
                
                instruction.Status = InstructionStatus.Paused;
                instruction.ErrorText = order.ErrorText;
                instruction.ShouldSendFailEmail = true;
                instruction.FailureTime = DateTime.UtcNow;
                
                await _repository.UpsertInstructions(instruction);
            }
            
            async Task UpdateInstructionSuccess(InvestOrder order)
            {
                var instruction = await _repository.GetInstructionById(order.InvestInstructionId);
                
                instruction.AvgPrice = (instruction.AvgPrice + order.Price)/2;
                instruction.TotalFromAmount += order.FromAmount;
                instruction.TotalToAmount += order.ToAmount;
                instruction.LastToAmount = order.ToAmount;
                
                await _repository.UpsertInstructions(instruction);
            }
        }

        private ErrorCode GetErrorCode(string quoteErrorMessage)
        {
            if (string.IsNullOrWhiteSpace(quoteErrorMessage))
                return ErrorCode.NoError;
            if (quoteErrorMessage.Contains("Asset pair do not supported"))
                return ErrorCode.PairNotSupported;
            if (quoteErrorMessage.Contains("Not enough balance"))
                return ErrorCode.LowBalance;

            return ErrorCode.InternalServerError;
        }

        private async Task SendFailureEmails()
        {
            try
            {
                if(DateTime.UtcNow.Hour != 12)
                    return;
                
                var instructions = await _repository.GetInstructionsByStatus(InstructionStatus.Active);
                foreach (var instruction in instructions.Where(t=>t.ShouldSendFailEmail))
                {
                    if (instruction.FailureTime.Day != DateTime.UtcNow.Day)
                    {
                        var pd = await _personalData.GetByIdAsync(new GetByIdRequest
                        {
                            Id = instruction.ClientId
                        });
                        if (pd.PersonalData == null) 
                            continue;
                        
                        var emailResponse = await _emailSender.SendRecurringBuyFailedEmailAsync(
                            new RecurrentBuyFailedGrpcRequestContract
                            {
                                Brand = pd.PersonalData.BrandId,
                                Lang = "En",
                                Platform = pd.PersonalData.PlatformType,
                                Email = pd.PersonalData.Email,
                                ToAsset = instruction.ToAsset,
                                FailureReason = instruction.ErrorText,
                                FromAmount = instruction.FromAmount,
                                FromAsset = instruction.FromAsset,
                                FailTime = instruction.FailureTime.ToString("R")
                            });
                        
                        if (!emailResponse.Result) 
                            continue;
                        
                        instruction.ShouldSendFailEmail = false;
                        await _repository.UpsertInstructions(instruction);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When sending failed instruction emails");
            }        
        }
        private static bool IsExpired(InvestInstruction instruction)
        {
            //return instruction.LastExecutionTime < DateTime.UtcNow.AddMinutes(-3);
            return instruction.ScheduleType switch
            {
                ScheduleType.Daily => instruction.LastExecutionTime < DateTime.UtcNow.AddDays(-1),
                ScheduleType.Weekly => instruction.LastExecutionTime < DateTime.UtcNow.AddDays(-7),
                ScheduleType.Biweekly => instruction.LastExecutionTime < DateTime.UtcNow.AddDays(-14),
                ScheduleType.Monthly => instruction.LastExecutionTime < DateTime.UtcNow.AddMonths(-1),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public void Start()
        {
            _timer.Start();
        }
    }
}