using BMPTec.BancoChuSa.API.Domain.Entities.BankTransfer;
using BMPTec.BancoChuSa.API.DTOs.BankTransfer;

namespace BMPTec.BancoChuSa.API.Application.Interfaces.Services
{
    public interface IBankTransferService
    {
        BankTransfer Transfer(
            Guid fromAccountId,
            Guid toAccountId,
            decimal amount,
            string? description = null);

        BankStatementResponse GetStatement(
            Guid accountId,
            DateTime startDate,
            DateTime endDate);

    }
}
