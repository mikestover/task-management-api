namespace TaskManagementApi.DTOs
{
    public class InvestmentResponseDto
    {
        public int Id { get; set; }
        public string Abbreviation { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public string? Category { get; set; }
        public int Shares { get; set; }
        public decimal Price { get; set; }
        public decimal InvestmentValue => Shares * Price;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
