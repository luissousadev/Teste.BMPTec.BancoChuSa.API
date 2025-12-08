using System.Data;
using BMPTec.BancoChuSa.API.Domain.Entities.BankTransfer;
using BMPTec.BancoChuSa.API.Domain.Interfaces.Repositories;
using Dapper;
using Npgsql;

namespace BMPTec.BancoChuSa.API.Infrastructure.Persistence
{
    public class BankTransferRepository: IBankTransferRepository
    {
        private readonly string _connectionString;

        public BankTransferRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
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

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                // 1) Buscar saldo da conta de origem e travar linha
                const string selectBalanceSql = @"
                SELECT balance
                FROM bank_accounts
                WHERE id = @Id
                FOR UPDATE; -- trava linha até fim da transação
            ";

                var fromBalance = connection.QueryFirstOrDefault<decimal?>(
                    selectBalanceSql,
                    new { Id = fromAccountId },
                    transaction: transaction
                );

                if(fromBalance is null)
                    throw new KeyNotFoundException("Conta de origem não encontrada.");

                if(fromBalance.Value < amount)
                    throw new InvalidOperationException("Saldo insuficiente na conta de origem.");

                // 2) Verificar se conta de destino existe
                var toBalance = connection.QueryFirstOrDefault<decimal?>(
                    selectBalanceSql,
                    new { Id = toAccountId },
                    transaction: transaction
                );

                if(toBalance is null)
                    throw new KeyNotFoundException("Conta de destino não encontrada.");

                // 3) Debitar conta de origem
                const string debitSql = @"
                UPDATE bank_accounts
                SET balance = balance - @Amount
                WHERE id = @Id;
            ";

                connection.Execute(
                    debitSql,
                    new { Id = fromAccountId, Amount = amount },
                    transaction: transaction
                );

                // 4) Creditar conta de destino
                const string creditSql = @"
                UPDATE bank_accounts
                SET balance = balance + @Amount
                WHERE id = @Id;
            ";

                connection.Execute(
                    creditSql,
                    new { Id = toAccountId, Amount = amount },
                    transaction: transaction
                );

                // 5) Registrar transferência na tabela bank_transfers
                const string insertTransferSql = @"
                INSERT INTO bank_transfers (
                    id,
                    from_account_id,
                    to_account_id,
                    amount,
                    description,
                    created_at
                )
                VALUES (
                    @Id,
                    @FromAccountId,
                    @ToAccountId,
                    @Amount,
                    @Description,
                    @CreatedAt
                )
                RETURNING 
                    id,
                    from_account_id AS FromAccountId,
                    to_account_id   AS ToAccountId,
                    amount,
                    description,
                    created_at;
            ";

                var transfer = new BankTransfer
                {
                    Id = Guid.NewGuid(),
                    FromAccountId = fromAccountId,
                    ToAccountId = toAccountId,
                    Amount = amount,
                    Description = description,
                    CreatedAt = DateTime.UtcNow
                };

                var createdTransfer = connection.QuerySingle<BankTransfer>(
                    insertTransferSql,
                    transfer,
                    transaction: transaction
                );

                transaction.Commit();

                return createdTransfer;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public List<BankTransfer> GetByAccountAndPeriod(Guid accountId, DateTime startDate, DateTime endDate)
        {
            const string sql = @"
                        SELECT
                            id,
                            from_account_id AS FromAccountId,
                            to_account_id   AS ToAccountId,
                            amount,
                            description,
                            created_at      AS CreatedAt
                        FROM bank_transfers
                        WHERE
                            (from_account_id = @AccountId OR to_account_id = @AccountId)
                            AND created_at >= @StartDate
                            AND created_at <= @EndDate
                        ORDER BY created_at ASC;
    ";

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            var transfers = connection.Query<BankTransfer>(
                sql,
                new { AccountId = accountId, StartDate = startDate, EndDate = endDate }
            ).ToList();

            return transfers;
        }

    }
}
