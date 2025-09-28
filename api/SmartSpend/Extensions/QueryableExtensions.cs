using Microsoft.EntityFrameworkCore;
using SmartSpend.Dtos;

namespace SmartSpend.Extensions
{
    public static class QueryableExtensions
    {
        public static async Task<PaginatedResponseDto<T>> ToPaginatedListAsync<T>(
            this IQueryable<T> query, int pageNumber, int pageSize)
        {
            var totalCount = await query.CountAsync(); // Get total number of items
            var items = await query.Skip((pageNumber - 1) * pageSize) // Skip previous pages
                                   .Take(pageSize) // Take only required items
                                   .ToListAsync(); // Convert to list

            return new PaginatedResponseDto<T>(items, totalCount, pageNumber, pageSize);
        }
    }
}
