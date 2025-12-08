namespace BMPTec.BancoChuSa.API.DTOs.BankAccount
{
    public class CreateBankAccountRequest
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string BankCode { get; set; } = string.Empty;   
        public string BankName { get; set; } = string.Empty;   
        public string BankBranch { get; set; } = string.Empty;   
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountDigit { get; set; } = string.Empty;
        public decimal Balance { get; set; } = 0;
        public DateTime CreatedAt { get; set; }
    }
}
