namespace BMPTec.BancoChuSa.API.DTOs.BankTransfer
{
    public class CreateBankTransferRequest
    {
        public Guid FromAccountId { get; set; }
        public Guid ToAccountId { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
    }
}
