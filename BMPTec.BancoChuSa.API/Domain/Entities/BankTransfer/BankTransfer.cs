namespace BMPTec.BancoChuSa.API.Domain.Entities.BankTransfer
{
    public class BankTransfer
    {
        public Guid Id { get; set; }
        public Guid FromAccountId { get; set; }
        public Guid ToAccountId { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
