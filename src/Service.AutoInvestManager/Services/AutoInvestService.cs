using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service;
using Service.AutoInvestManager.Domain.Models;
using Service.AutoInvestManager.Grpc;
using Service.AutoInvestManager.Grpc.Models;
using Service.AutoInvestManager.Helpers;

namespace Service.AutoInvestManager.Services
{
    public class AutoInvestService : IAutoInvestService
    {
        private readonly ILogger<AutoInvestService> _logger;
        private readonly InstructionsRepository _repository;

        public AutoInvestService(ILogger<AutoInvestService> logger, InstructionsRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }


        public async Task<OperationResponse> CreateInstruction(CreateInstructionRequest request)
        {
            try
            {
                _logger.LogInformation("Creating invest instruction. Request {reqest}", request.ToJson());
                var instruction = new InvestInstruction
                {
                    Id = Guid.NewGuid().ToString("N"),
                    ClientId = request.ClientId,
                    BrokerId = request.BrokerId,
                    WalletId = request.WalletId,
                    FromAmount = request.FromAmount,
                    FromAsset = request.FromAsset,
                    ToAsset = request.ToAsset,
                    Status = InstructionStatus.Active,
                    ScheduleType = request.ScheduleType,
                    ScheduledTime = TimeOnly.FromDateTime(request.ScheduledTime),
                    ScheduledDayOfWeek = request.ScheduledDayOfWeek,
                    ScheduledDayOfMonth = request.ScheduledDayOfMonth,
                    CreationTime = DateTime.UtcNow,
                    LastExecutionTime = request.ExecuteImmediately ? DateTime.MinValue : DateTime.UtcNow,
                    ShouldSendFailEmail = false
                };
                await _repository.UpsertInstructions(instruction);
                await _repository.UpsertInstructionAudit(instruction);

                return new OperationResponse
                {
                    IsSuccess = true
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
                instruction.ScheduledTime = TimeOnly.FromDateTime(request.ScheduledTime);
                instruction.ScheduledDayOfWeek = request.ScheduledDayOfWeek;
                instruction.ScheduledDayOfMonth = request.ScheduledDayOfMonth;
                instruction.LastExecutionTime = DateTime.MinValue;
                instruction.ShouldSendFailEmail = false;
                    
                await _repository.UpsertInstructions(instruction);
                await _repository.UpsertInstructionAudit(instruction);

                return new OperationResponse
                {
                    IsSuccess = true
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
                    IsSuccess = true
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
                    IsSuccess = true
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