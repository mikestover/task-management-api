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
    public class InvestmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public InvestmentsController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: api/investments
        [HttpGet]
        public async Task<ActionResult<PagedResult<InvestmentResponseDto>>> GetInvestments([FromQuery] InvestmentQueryParameters queryParams)
        {
            // Start with all investments
            var query = _context.Investments.AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(queryParams.Category))
            {
                query = query.Where(i => i.Category == queryParams.Category);
            }

            // Apply search
            if (!string.IsNullOrWhiteSpace(queryParams.SearchTerm))
            {
                var searchTerm = queryParams.SearchTerm.ToLower();
                query = query.Where(i =>
                    i.Abbreviation.ToLower().Contains(searchTerm) ||
                    (i.Notes != null && i.Notes.ToLower().Contains(searchTerm))
                );
            }

            // Apply sorting
            query = queryParams.SortBy?.ToLower() switch
            {
                "abbreviation" => queryParams.SortDescending ? query.OrderByDescending(i => i.Abbreviation) : query.OrderBy(i => i.Abbreviation),
                "price" => queryParams.SortDescending ? query.OrderByDescending(i => i.Price) : query.OrderBy(i => i.Price),
                "shares" => queryParams.SortDescending ? query.OrderByDescending(i => i.Shares) : query.OrderBy(i => i.Shares),
                "category" => queryParams.SortDescending ? query.OrderByDescending(i => i.Category) : query.OrderBy(i => i.Category),
                "updatedat" => queryParams.SortDescending ? query.OrderByDescending(i => i.UpdatedAt) : query.OrderBy(i => i.UpdatedAt),
                _ => queryParams.SortDescending ? query.OrderByDescending(i => i.CreatedAt) : query.OrderBy(i => i.CreatedAt)
            };

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            var investments = await query
                .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .ToListAsync();

            // Map to DTOs
            var investmentDtos = investments.Select(i => MapToResponseDto(i)).ToList();

            // Create paginated result
            var result = new PagedResult<InvestmentResponseDto>
            {
                Items = investmentDtos,
                PageNumber = queryParams.PageNumber,
                PageSize = queryParams.PageSize,
                TotalCount = totalCount
            };

            return Ok(result);
        }

        // GET: api/investments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<InvestmentResponseDto>> GetInvestment(int id)
        {
            var investment = await _context.Investments.FindAsync(id);

            if (investment == null)
            {
                return NotFound();
            }

            return Ok(MapToResponseDto(investment));
        }

        // POST: api/investments
        [HttpPost]
        public async Task<ActionResult<InvestmentResponseDto>> CreateInvestment(CreateInvestmentDto createDto)
        {
            var validator = new CreateInvestmentDtoValidator();
            var validationResult = await validator.ValidateAsync(createDto);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var investment = new Investment
            {
                Abbreviation = createDto.Abbreviation,
                Notes = createDto.Notes,
                Category = createDto.Category,
                Shares = createDto.Shares,
                Price = createDto.Price,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Investments.Add(investment);
            await _context.SaveChangesAsync();

            var response = MapToResponseDto(investment);
            return CreatedAtAction(nameof(GetInvestment), new { id = investment.Id }, response);
        }

        // PUT: api/investments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInvestment(int id, UpdateInvestmentDto updateDto)
        {
            var validator = new UpdateInvestmentDtoValidator();
            var validationResult = await validator.ValidateAsync(updateDto);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var investment = await _context.Investments.FindAsync(id);

            if (investment == null)
            {
                return NotFound();
            }

            investment.Abbreviation = updateDto.Abbreviation;
            investment.Notes = updateDto.Notes;
            investment.Category = updateDto.Category;
            investment.Shares = updateDto.Shares;
            investment.Price = updateDto.Price;
            investment.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InvestmentExists(id))
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

        // DELETE: api/investments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvestment(int id)
        {
            var investment = await _context.Investments.FindAsync(id);
            if (investment == null)
            {
                return NotFound();
            }

            _context.Investments.Remove(investment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool InvestmentExists(int id)
        {
            return _context.Investments.Any(e => e.Id == id);
        }

        private static InvestmentResponseDto MapToResponseDto(Investment investment)
        {
            return new InvestmentResponseDto
            {
                Id = investment.Id,
                Abbreviation = investment.Abbreviation,
                Notes = investment.Notes,
                Category = investment.Category,
                Shares = investment.Shares,
                Price = investment.Price,
                CreatedAt = investment.CreatedAt,
                UpdatedAt = investment.UpdatedAt
            };
        }
    }
}
