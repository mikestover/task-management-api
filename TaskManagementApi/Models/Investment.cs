namespace TaskManagementApi.Models;

public class Investment
{
    public int Id { get; set; }
    public required string Abbreviation { get; set; }
    public string? Notes { get; set; }
    public string? Category { get; set; }
    public int Shares { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
