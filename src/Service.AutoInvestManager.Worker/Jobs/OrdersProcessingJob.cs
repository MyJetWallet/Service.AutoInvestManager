using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service;
using MyJetWallet.Sdk.Service.Tools;
using MyJetWallet.Sdk.ServiceBus;
using MyServiceBus.Abstractions;
using Service.AutoInvestManager.Domain.Helpers;
using Service.AutoInvestManager.Domain.Models;
using Service.EmailSender.Grpc;
using Service.EmailSender.Grpc.Models;
using Service.Liquidity.Converter.Domain.Models;
using Service.Liquidity.Converter.Grpc;
using Service.Liquidity.Converter.Grpc.Models;

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
        
        public OrdersProcessingJob(ILogger<OrdersProcessingJob> logger, InstructionsRepository repository, IQuoteService quoteService, IServiceBusPublisher<InvestOrder> publisher, IEmailSenderService emailSender)
        {
            _logger = logger;
            _repository = repository;
            _quoteService = quoteService;
            _publisher = publisher;
            _emailSender = emailSender;
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
                        RecurringBuy = null
                    });
                    if (!quote.IsSuccess)
                    {
                        order.Status = OrderStatus.Failed;
                        order.ErrorText = quote.ErrorMessage;
                        await _repository.UpsertOrders(order);
                        await SetInstructionAsFailed(order);
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

                    order.QuoteId = quoteResult.Data.OperationId;
                    
                    if (quoteResult.QuoteExecutionResult == QuoteExecutionResult.Success)
                    {
                        order.ExecutionTime = DateTime.UtcNow;
                        order.Price = quoteResult.Data.Price;
                        order.ToAmount = quote.Data.ToAssetVolume;
                        order.Status = OrderStatus.Executed;
                    }

                    if (quoteResult.QuoteExecutionResult == QuoteExecutionResult.Error || quoteResult.QuoteExecutionResult == QuoteExecutionResult.ReQuote)
                    {
                        order.Status = OrderStatus.Failed;
                        order.ErrorText = quoteResult.ErrorMessage;
                        await SetInstructionAsFailed(order);
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
            async Task SetInstructionAsFailed(InvestOrder order)
            {
                var instruction = await _repository.GetInstructionById(order.InvestInstructionId);
                
                instruction.Status = InstructionStatus.Paused;
                instruction.ErrorText = order.ErrorText;
                instruction.ShouldSendFailEmail = true;
                instruction.FailureTime = DateTime.UtcNow;
                
                await _repository.UpsertInstructions(instruction);
            }
        }
        
        private async Task SendFailureEmails()
        {
            try
            {
                if(DateTime.UtcNow.Hour != 12)
                    return;
                
                var instructions = await _repository.GetInstructionsByStatus(InstructionStatus.Paused);
                foreach (var instruction in instructions.Where(t=>t.ShouldSendFailEmail))
                {
                    if (instruction.FailureTime.Day != DateTime.UtcNow.Day)
                    {
                        //TODO: Send email
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