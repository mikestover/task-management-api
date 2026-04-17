namespace TaskManagementApi.Models
{
    public class InvestmentQueryParameters
    {
        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        
        // Filtering
        public string? Category { get; set; }
        
        // Sorting
        public string? SortBy { get; set; } = "CreatedAt";
        public bool SortDescending { get; set; } = true;
        
        // Search
        public string? SearchTerm { get; set; }
    }
}
