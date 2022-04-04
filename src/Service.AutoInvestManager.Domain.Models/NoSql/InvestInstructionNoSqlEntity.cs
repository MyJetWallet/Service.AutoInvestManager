using MyNoSqlServer.Abstractions;

namespace Service.AutoInvestManager.Domain.Models.NoSql
{
    public class InvestInstructionNoSqlEntity: MyNoSqlDbEntity
    {
        public const string TableName = "myjetwallet-autoinvest-instructions";

        public static string GeneratePartitionKey(string clientId) => clientId;
        public static string GenerateRowKey(string instructionId) => instructionId;

        public InvestInstruction Instruction { get; set; }

        public static InvestInstructionNoSqlEntity Create(InvestInstruction instruction)
        {
            return new InvestInstructionNoSqlEntity()
            {
                PartitionKey = GeneratePartitionKey(instruction.ClientId),
                RowKey = GenerateRowKey(instruction.Id),
                Instruction = instruction
            };
        }
    }
}