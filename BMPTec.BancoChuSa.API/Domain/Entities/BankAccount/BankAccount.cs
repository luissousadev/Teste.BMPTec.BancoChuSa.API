namespace BMPTec.BancoChuSa.API.Domain.Entities.BankAccount
{
    public class BankAccount
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string BankCode { get; set; } = string.Empty;   // Ex: 001, 237, 341
        public string BankName { get; set; } = string.Empty;   // Ex: Banco do Brasil
        public string BankBranch { get; set; } = string.Empty;     // Agência
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountDigit { get; set; } = string.Empty;
        public decimal Balance { get; set; } = 0;
        public DateTime CreatedAt { get; set; }
    }
}
