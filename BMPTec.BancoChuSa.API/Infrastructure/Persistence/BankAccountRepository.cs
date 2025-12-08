using BMPTec.BancoChuSa.API.Domain.Entities.BankAccount;
using BMPTec.BancoChuSa.API.Domain.Interfaces.Repositories;
using Dapper;
using Npgsql;
using System.Data;

namespace BMPTec.BancoChuSa.API.Infrastructure.Persistence
{
    public class BankAccountRepository: IBankAccountRepository
    {
        private readonly string _connectionString;

        public BankAccountRepository(IConfiguration configuration)
        {            
            _connectionString = configuration.GetConnectionString("DefaultConnection");                
        }

        public BankAccount Create(BankAccount account)
        {

            const string sql = @"
                INSERT INTO bank_accounts (
                    id,
                    customer_id,
                    bank_code,
                    bank_name,
                    bank_branch,
                    account_number,
                    account_digit,
                    created_at
                )
                VALUES (
                    @Id,
                    @CustomerId,
                    @BankCode,
                    @BankName,
                    @BankBranch,
                    @AccountNumber,
                    @AccountDigit,
                    @CreatedAt
                )
                RETURNING 
                    id,
                    customer_id AS CustomerId,
                    bank_code AS BankCode,
                    bank_name AS BankName,
                    bank_branch AS BankBranch,
                    account_number AS AccountNumber,
                    account_digit AS AccountDigit,
                    created_at AS CreatedAt;
            ";

            using NpgsqlConnection connection = new NpgsqlConnection(_connectionString);             
            connection.Open();

            var created = connection.QuerySingle<BankAccount>(sql, account);
            return created;
        }

        public BankAccount? GetByIdAsync(Guid id)
        {
            const string sql = @"
                SELECT
                    id,
                    customer_id AS CustomerId,
                    bank_code AS BankCode,
                    bank_name AS BankName,
                    bank_branch AS BankBranch,
                    account_number AS AccountNumber,
                    account_digit AS AccountDigit,
                    created_at AS CreatedAt
                FROM bank_accounts
                WHERE id = @Id;
            ";

            using NpgsqlConnection connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            var account = connection.QueryFirstOrDefault<BankAccount>(sql, new { Id = id });
            return account; // pode ser null
        }
    }
}
