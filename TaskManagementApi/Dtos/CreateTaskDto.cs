using System.ComponentModel.DataAnnotations;
using TaskManagementApi.Models;

namespace TaskManagementApi.DTOs
{
    public class CreateTaskDto
    {
        [Required]
        [StringLength(200)]
        public required string Title { get; set; }
        
        [StringLength(1000)]
        public string? Description { get; set; }
        
        public Priority Priority { get; set; } = Priority.Medium;
        
        public DateTime? DueDate { get; set; }
    }
}