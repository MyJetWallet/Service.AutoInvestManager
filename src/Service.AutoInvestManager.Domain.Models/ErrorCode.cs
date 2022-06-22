namespace Service.AutoInvestManager.Domain.Models
{
    public enum ErrorCode
    {
        NoError = 0,
        LowBalance = 1,
        PairNotSupported = 2,
        InternalServerError = 3,
    }
}