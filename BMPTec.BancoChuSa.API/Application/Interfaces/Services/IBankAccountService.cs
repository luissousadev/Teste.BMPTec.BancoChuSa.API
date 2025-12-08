using BMPTec.BancoChuSa.API.Domain.Entities.BankAccount;

namespace BMPTec.BancoChuSa.API.Application.Interfaces.Services
{
    public interface IBankAccountService
    {
        BankAccount Create(BankAccount account);
        BankAccount? GetById(Guid id);
    }
}
