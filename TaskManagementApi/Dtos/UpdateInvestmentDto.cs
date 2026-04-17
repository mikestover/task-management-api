using System.ComponentModel.DataAnnotations;

namespace TaskManagementApi.DTOs
{
    public class UpdateInvestmentDto
    {
        [Required]
        [StringLength(200)]
        public required string Abbreviation { get; set; }
        
        [StringLength(1000)]
        public string? Notes { get; set; }
        
        [StringLength(200)]
        public string? Category { get; set; }
        
        public int Shares { get; set; }
        
        public decimal Price { get; set; }
        
        public decimal InvestmentValue => Shares * Price;
    }
}
