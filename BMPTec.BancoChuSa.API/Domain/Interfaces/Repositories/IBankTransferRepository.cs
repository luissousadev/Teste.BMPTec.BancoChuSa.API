using BMPTec.BancoChuSa.API.Domain.Entities.BankTransfer;

namespace BMPTec.BancoChuSa.API.Domain.Interfaces.Repositories
{
    public interface IBankTransferRepository
    { 
        BankTransfer Transfer(
            Guid fromAccountId,
            Guid toAccountId,
            decimal amount,
            string? description = null);

        List<BankTransfer> GetByAccountAndPeriod(
        Guid accountId,
        DateTime startDate,
        DateTime endDate);
    }
}
