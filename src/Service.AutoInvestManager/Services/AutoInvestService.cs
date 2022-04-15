using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service;
using Service.AutoInvestManager.Domain.Helpers;
using Service.AutoInvestManager.Domain.Models;
using Service.AutoInvestManager.Grpc;
using Service.AutoInvestManager.Grpc.Models;
using Service.Liquidity.Converter.Grpc;
using Service.Liquidity.Converter.Grpc.Models;

namespace Service.AutoInvestManager.Services
{
    public class AutoInvestService : IAutoInvestService
    {
        private readonly ILogger<AutoInvestService> _logger;
        private readonly InstructionsRepository _repository;
        private readonly IHistoryService _quoteHistoryService;
        private readonly IQuoteService _quoteService;
        public AutoInvestService(ILogger<AutoInvestService> logger, InstructionsRepository repository, IHistoryService quoteHistoryService, IQuoteService quoteService)
        {
            _logger = logger;
            _repository = repository;
            _quoteHistoryService = quoteHistoryService;
            _quoteService = quoteService;
        }

        private async Task<InvestInstruction> CreateInvestInstruction(CreateInstructionRequest request)
        {
            
            var quoteResponse = await _quoteHistoryService.GetQuoteById(new GetQuoteByIdRequest
            {
                QuoteId = request.QuoteId
            });
            var t = _quoteService.GetQuoteAsync(new GetQuoteRequest());
            if (!quoteResponse.Success)
                throw new Exception("Quote not found");

            return new InvestInstruction
            {
                Id = Guid.NewGuid().ToString("N"),
                ClientId = request.ClientId,
                BrokerId = request.BrokerId,
                WalletId = request.WalletId,
                FromAmount = quoteResponse.Quote.FromAssetVolume,
                FromAsset = quoteResponse.Quote.FromAsset,
                ToAsset = quoteResponse.Quote.ToAsset,
                Status = InstructionStatus.Active,
                ScheduleType = request.ScheduleType,
                ScheduledDateTime = DateTime.UtcNow,
                ScheduledDayOfWeek = DateTime.UtcNow.DayOfWeek,
                ScheduledDayOfMonth = DateTime.DaysInMonth(DateTime.UtcNow.Year, DateTime.UtcNow.Month),
                CreationTime = DateTime.UtcNow,
                LastExecutionTime = quoteResponse.Quote.OpenDate,
                ShouldSendFailEmail = false,
                OriginalQuoteId = request.QuoteId
            };
        }

        public async Task<CreateInstructionResponse> PreviewInstruction(CreateInstructionRequest request)
        {
            try
            {
                var instruction = await CreateInvestInstruction(request);
                var nextExecTime = instruction.ScheduleType switch
                {
                    ScheduleType.Daily => instruction.LastExecutionTime.AddDays(1),
                    ScheduleType.Weekly => instruction.LastExecutionTime.AddDays(7),
                    ScheduleType.Biweekly => instruction.LastExecutionTime.AddDays(14),
                    ScheduleType.Monthly => instruction.LastExecutionTime.AddMonths(1),
                    _ => throw new ArgumentOutOfRangeException()
                };
                return new CreateInstructionResponse
                {
                    IsSuccess = true,
                    Instruction = instruction,
                    NextExecutionDate = nextExecTime 
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "At previewing invest instruction");
                return new CreateInstructionResponse
                {
                    IsSuccess = false,
                    ErrorMessage = e.Message
                };
            }
        }

        public async Task<CreateInstructionResponse> CreateInstruction(CreateInstructionRequest request)
        {
            try
            {
                _logger.LogInformation("Creating invest instruction. Request {reqest}", request.ToJson());

                var instruction = await CreateInvestInstruction(request);
                
                var nextExecTime = instruction.ScheduleType switch
                {
                    ScheduleType.Daily => instruction.LastExecutionTime.AddDays(1),
                    ScheduleType.Weekly => instruction.LastExecutionTime.AddDays(7),
                    ScheduleType.Biweekly => instruction.LastExecutionTime.AddDays(14),
                    ScheduleType.Monthly => instruction.LastExecutionTime.AddMonths(1),
                    _ => throw new ArgumentOutOfRangeException()
                };
                
                await _repository.UpsertInstructions(instruction);
                await _repository.UpsertInstructionAudit(instruction);

                return new CreateInstructionResponse
                {
                    IsSuccess = true,
                    Instruction = instruction, 
                    NextExecutionDate = nextExecTime
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "At switching invest instruction");
                return new CreateInstructionResponse
                {
                    IsSuccess = false,
                    ErrorMessage = e.Message
                };
            }
        }

        public async Task<OperationResponse> UpdateInstruction(UpdateInstructionRequest request)
        {
            try
            {
                _logger.LogInformation("Updating invest instruction status. Request {reqest}", request.ToJson());

                var instruction = await _repository.GetInvestInstructionById(request.InstructionId);
                if (instruction == null)
                    return new OperationResponse
                    {
                        IsSuccess = true
                    };

                if (instruction.ClientId != request.ClientId)
                    return new OperationResponse
                    {
                        IsSuccess = true
                    };
                
                instruction.FromAsset = request.FromAsset;
                instruction.FromAmount = request.FromAmount;
                instruction.Status = InstructionStatus.Active;
                instruction.ScheduleType = request.ScheduleType;
                instruction.ScheduledDateTime = request.ScheduledTime;
                instruction.ScheduledDayOfWeek = request.ScheduledDayOfWeek;
                instruction.ScheduledDayOfMonth = request.ScheduledDayOfMonth;
                instruction.LastExecutionTime = DateTime.MinValue;
                instruction.ShouldSendFailEmail = false;
                    
                await _repository.UpsertInstructions(instruction);
                await _repository.UpsertInstructionAudit(instruction);

                return new OperationResponse
                {
                    IsSuccess = true,
                    Instruction = instruction
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "At updating invest instruction");
                return new OperationResponse
                {
                    IsSuccess = false,
                    ErrorMessage = e.Message
                };
            }
        }

        public async Task<OperationResponse> RemoveInstruction(RemoveInstructionRequest request)
        {
            try
            {
                _logger.LogInformation("Deleting invest instruction status. Request {reqest}", request.ToJson());

                var instruction = await _repository.GetInvestInstructionById(request.InstructionId);
                if (instruction == null)
                    return new OperationResponse
                    {
                        IsSuccess = true
                    };

                if (instruction.ClientId != request.ClientId)
                    return new OperationResponse
                    {
                        IsSuccess = true
                    };

                instruction.Status = InstructionStatus.Deleted;
                await _repository.UpsertInstructions(instruction);
                await _repository.UpsertInstructionAudit(instruction);

                return new OperationResponse
                {
                    IsSuccess = true,
                    Instruction = instruction
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "At deleting invest instruction");
                return new OperationResponse
                {
                    IsSuccess = false,
                    ErrorMessage = e.Message
                };
            }
        }

        public async Task<OperationResponse> SwitchInstruction(SwitchInstructionRequest request)
        {
            try
            {
                _logger.LogInformation("Switching invest instruction status. Request {reqest}", request.ToJson());

                var instruction = await _repository.GetInvestInstructionById(request.InstructionId);
                if (instruction == null)
                    return new OperationResponse
                    {
                        IsSuccess = false,
                        ErrorMessage = "Instruction not found"
                    };

                if (instruction.ClientId != request.ClientId)
                    return new OperationResponse
                    {
                        IsSuccess = false,
                        ErrorMessage = "Instruction not owned by this user"
                    };

                instruction.Status = request.Status;
                await _repository.UpsertInstructions(instruction);
                await _repository.UpsertInstructionAudit(instruction);

                return new OperationResponse
                {
                    IsSuccess = true,
                    Instruction = instruction
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "At switching invest instruction");
                return new OperationResponse
                {
                    IsSuccess = false,
                    ErrorMessage = e.Message
                };
            }
        }

        public async Task<GetOrdersResponse> GetOrders(GetOrdersRequest request)
        {
            _logger.LogInformation("Getting orders. Request {reqest}", request.ToJson());
            var orders = await _repository.GetInvestOrders(request.SearchText, request.Take, request.LastSeen);
            return new GetOrdersResponse
            {
                Orders = orders
            };
        }

        public async Task<GetInstructionResponse> GetInstructions(GetInstructionRequest request)
        {
            _logger.LogInformation("Getting instruction. Request {reqest}", request.ToJson());
            var instruction =
                await _repository.GetInvestInstructions(request.SearchText, request.Take, request.LastSeen);
            return new GetInstructionResponse
            {
                Instructions = instruction
            };
        }
    }
}