using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementApi.Data;
using TaskManagementApi.Models;
using TaskManagementApi.DTOs;
using TaskManagementApi.Validators;

namespace TaskManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TasksController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: api/tasks
        [HttpGet]
        public async Task<ActionResult<PagedResult<TaskResponseDto>>> GetTasks([FromQuery] TaskQueryParameters queryParams)
        {
            // Start with all tasks
            var query = _context.Tasks.AsQueryable();

            // Apply filters
            if (queryParams.IsCompleted.HasValue)
            {
                query = query.Where(t => t.IsCompleted == queryParams.IsCompleted.Value);
            }

            if (queryParams.Priority.HasValue)
            {
                query = query.Where(t => t.Priority == queryParams.Priority.Value);
            }

            if (queryParams.DueDateFrom.HasValue)
            {
                query = query.Where(t => t.DueDate >= queryParams.DueDateFrom.Value);
            }

            if (queryParams.DueDateTo.HasValue)
            {
                query = query.Where(t => t.DueDate <= queryParams.DueDateTo.Value);
            }

            // Apply search
            if (!string.IsNullOrWhiteSpace(queryParams.SearchTerm))
            {
                var searchTerm = queryParams.SearchTerm.ToLower();
                query = query.Where(t =>
                    t.Title.ToLower().Contains(searchTerm) ||
                    (t.Description != null && t.Description.ToLower().Contains(searchTerm))
                );
            }

            // Apply sorting
            query = queryParams.SortBy?.ToLower() switch
            {
                "title" => queryParams.SortDescending ? query.OrderByDescending(t => t.Title) : query.OrderBy(t => t.Title),
                "priority" => queryParams.SortDescending ? query.OrderByDescending(t => t.Priority) : query.OrderBy(t => t.Priority),
                "duedate" => queryParams.SortDescending ? query.OrderByDescending(t => t.DueDate) : query.OrderBy(t => t.DueDate),
                "iscompleted" => queryParams.SortDescending ? query.OrderByDescending(t => t.IsCompleted) : query.OrderBy(t => t.IsCompleted),
                "updatedat" => queryParams.SortDescending ? query.OrderByDescending(t => t.UpdatedAt) : query.OrderBy(t => t.UpdatedAt),
                _ => queryParams.SortDescending ? query.OrderByDescending(t => t.CreatedAt) : query.OrderBy(t => t.CreatedAt)
            };

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            var tasks = await query
                .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .ToListAsync();

            // Map to DTOs
            var taskDtos = tasks.Select(t => MapToResponseDto(t)).ToList();

            // Create paginated result
            var result = new PagedResult<TaskResponseDto>
            {
                Items = taskDtos,
                PageNumber = queryParams.PageNumber,
                PageSize = queryParams.PageSize,
                TotalCount = totalCount
            };

            return Ok(result);
        }

        // GET: api/tasks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskResponseDto>> GetTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
            {
                return NotFound();
            }

            return Ok(MapToResponseDto(task));
        }

        // POST: api/tasks
        [HttpPost]
        public async Task<ActionResult<TaskResponseDto>> CreateTask(CreateTaskDto createDto)
        {
            var validator = new CreateTaskDtoValidator();
            var validationResult = await validator.ValidateAsync(createDto);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var task = new TaskItem
            {
                Title = createDto.Title,
                Description = createDto.Description,
                Priority = createDto.Priority,
                DueDate = createDto.DueDate,
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            var response = MapToResponseDto(task);
            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, response);
        }

        // PUT: api/tasks/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, UpdateTaskDto updateDto)
        {
            var validator = new UpdateTaskDtoValidator();
            var validationResult = await validator.ValidateAsync(updateDto);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
            {
                return NotFound();
            }

            task.Title = updateDto.Title;
            task.Description = updateDto.Description;
            task.IsCompleted = updateDto.IsCompleted;
            task.Priority = updateDto.Priority;
            task.DueDate = updateDto.DueDate;
            task.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/tasks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TaskExists(int id)
        {
            return _context.Tasks.Any(e => e.Id == id);
        }

        private static TaskResponseDto MapToResponseDto(TaskItem task)
        {
            return new TaskResponseDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                IsCompleted = task.IsCompleted,
                Priority = task.Priority,
                DueDate = task.DueDate,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt
            };
        }
    }
}