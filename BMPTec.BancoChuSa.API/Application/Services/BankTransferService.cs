using BMPTec.BancoChuSa.API.Application.Interfaces.Services;
using BMPTec.BancoChuSa.API.Domain.Entities.BankTransfer;
using BMPTec.BancoChuSa.API.Domain.Interfaces.Repositories;
using BMPTec.BancoChuSa.API.DTOs.BankTransfer;
using Microsoft.Extensions.Caching.Memory;
using RestSharp;
using System.Text.Json;
using System.Threading;

namespace BMPTec.BancoChuSa.API.Application.Services
{
    public class BankTransferService: IBankTransferService
    {
        private readonly IBankTransferRepository _transferRepository;
        private readonly IMemoryCache _cache;

        public BankTransferService(IBankTransferRepository transferRepository, IMemoryCache cache)
        {
            _transferRepository = transferRepository;
            _cache = cache;
        }


        public BankTransfer Transfer(
            Guid fromAccountId,
            Guid toAccountId,
            decimal amount,
            string? description = null)
        {
            if(fromAccountId == Guid.Empty)
                throw new ArgumentException("Conta de origem inválida.", nameof(fromAccountId));

            if(toAccountId == Guid.Empty)
                throw new ArgumentException("Conta de destino inválida.", nameof(toAccountId));

            if(fromAccountId == toAccountId)
                throw new InvalidOperationException("Não é possível transferir para a mesma conta.");

            if(amount <= 0)
                throw new InvalidOperationException("O valor da transferência deve ser maior que zero.");
            
            var today = DateTime.Today;
            if(!IsBusinessDay(today))
            {
                throw new InvalidOperationException("Transferências só podem ser realizadas em dias úteis que não sejam feriados.");
            }

            // Lógica de débito/crédito + registro da transferência (repositório faz isso)
            var transfer = _transferRepository.Transfer(
                fromAccountId,
                toAccountId,
                amount,
                description);

            return transfer;
        }

        public BankStatementResponse GetStatement(Guid accountId, DateTime startDate, DateTime endDate)
        {
            if(accountId == Guid.Empty)
                throw new ArgumentException("Conta inválida.", nameof(accountId));

            if(startDate > endDate)
                throw new ArgumentException("Data inicial não pode ser maior que a data final.");

            var transfers = _transferRepository.GetByAccountAndPeriod(
                accountId,
                startDate,
                endDate);

            var items = new List<BankStatementItem>();

            foreach(var transfer in transfers)
            {
                var isDebit = transfer.FromAccountId == accountId;

                var item = new BankStatementItem
                {
                    Date = transfer.CreatedAt,
                    Amount = transfer.Amount,
                    Description = transfer.Description,
                    FromAccountId = transfer.FromAccountId,
                    ToAccountId = transfer.ToAccountId
                };

                items.Add(item);
            }

            return new BankStatementResponse
            {
                AccountId = accountId,
                StartDate = startDate,
                EndDate = endDate,
                Items = items
            };
        }

        private bool IsBusinessDay(DateTime date)
        {
            if(date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
                return false;

            var holidays = GetHolidays(date.Year);

            return !holidays.Any(h => DateTime.Parse(h.Date!).Date == date.Date);
        }

        private List<HolidayDto> GetHolidays(int year)
        {
            var cacheKey = $"holidays-{year}";

            // 🧠 Se já existe no cache → retorna imediatamente
            if(_cache.TryGetValue(cacheKey, out List<HolidayDto>? cached))
                return cached!;

            // 🛰️ Senão, chama API
            var client = new RestClient("https://brasilapi.com.br");
            var request = new RestRequest($"/api/feriados/v1/{year}", Method.Get);

            var response = client.Execute(request);

            if(!response.IsSuccessful || string.IsNullOrWhiteSpace(response.Content))
                throw new InvalidOperationException("Não foi possível validar feriados (API indisponível).");

            var holidays = JsonSerializer.Deserialize<List<HolidayDto>>(response.Content!, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new();

            // 🧠 Salvar no cache por 24h
            _cache.Set(cacheKey, holidays, TimeSpan.FromHours(24));

            return holidays;
        }

    }


    internal class HolidayDto
    {
        public string? Date { get; set; }   
        public string? Name { get; set; }
        public string? Type { get; set; }
    }
}
