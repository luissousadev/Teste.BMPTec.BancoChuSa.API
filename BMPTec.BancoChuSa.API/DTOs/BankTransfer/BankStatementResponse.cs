namespace BMPTec.BancoChuSa.API.DTOs.BankTransfer
{
    public class BankStatementResponse
    {
        public Guid AccountId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public List<BankStatementItem> Items { get; set; } = new();
    }
}
