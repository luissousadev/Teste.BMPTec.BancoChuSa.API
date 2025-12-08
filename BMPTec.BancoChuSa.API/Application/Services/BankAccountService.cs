using BMPTec.BancoChuSa.API.Application.Interfaces.Services;
using BMPTec.BancoChuSa.API.Domain.Entities.BankAccount;
using BMPTec.BancoChuSa.API.Domain.Interfaces.Repositories;

namespace BMPTec.BancoChuSa.API.Application.Services
{
    public class BankAccountService: IBankAccountService
    {
        private readonly IBankAccountRepository _repository;

        public BankAccountService(IBankAccountRepository repository)
        {
            _repository = repository;
        }

        public BankAccount Create(BankAccount account)
        {
            if(account is null)
                throw new ArgumentNullException(nameof(account));

            if(account.CustomerId == Guid.Empty)
                throw new ArgumentException("CustomerId não pode ser vazio.", nameof(account.CustomerId));

            // Garante que sempre terá Id e CreatedAt consistentes
            if(account.Id == Guid.Empty)
                account.Id = Guid.NewGuid();

            if(account.CreatedAt == default)
                account.CreatedAt = DateTime.UtcNow;

            var created = _repository.Create(account);
            return created;
        }

        public BankAccount? GetById(Guid id)
        {
            if(id == Guid.Empty)
                throw new ArgumentException("Id não pode ser vazio.", nameof(id));

            var account = _repository.GetByIdAsync(id);
            return account;
        }
    }
}
