using BMPTec.BancoChuSa.API.Domain.Entities.BankAccount;

namespace BMPTec.BancoChuSa.API.Domain.Interfaces.Repositories
{
    public interface IBankAccountRepository
    {
        BankAccount Create(BankAccount account);
        BankAccount? GetByIdAsync(Guid id);
    }
}
