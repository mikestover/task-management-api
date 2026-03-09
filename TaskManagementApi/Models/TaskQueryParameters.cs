namespace TaskManagementApi.Models
{
    public class TaskQueryParameters
    {
        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        
        // Filtering
        public bool? IsCompleted { get; set; }
        public Priority? Priority { get; set; }
        public DateTime? DueDateFrom { get; set; }
        public DateTime? DueDateTo { get; set; }
        
        // Sorting
        public string? SortBy { get; set; } = "CreatedAt";
        public bool SortDescending { get; set; } = true;
        
        // Search
        public string? SearchTerm { get; set; }
    }
}