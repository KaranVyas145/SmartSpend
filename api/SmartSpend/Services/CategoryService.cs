using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using SmartSpend.Data;
using SmartSpend.Dtos;
using SmartSpend.Extensions;
using SmartSpend.Models;

namespace SmartSpend.Services
{
    public interface ICategoryService
    {
        public Task<List<CategoryDto>> GetAllCategoriesAsync(); 
        public Task<PaginatedResponseDto<CategoryDto>> GetCategoriesListAsync(PaginationRequestDto paginationRequestDto);
        public Task<CategoryDto> GetCategoryByIdAsync(string Id);
        public Task<CategoryDto> CreateCategoryAsync(CategoryDto categoryDto);
        public Task<CategoryDto> UpdateCategoryAsync(string Id, CategoryDto categoryDto);
        public Task DeleteCategoryAsync(string Id);
    }

    public class CategoryService: ICategoryService
    {
        public readonly ILogger<CategoryService> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public CategoryService(ILogger<CategoryService> logger, ApplicationDbContext dbContext, IMapper mapper, IUserService userService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<List<CategoryDto>> GetAllCategoriesAsync()
        {
            var categoryList = await _dbContext.Categories.ToListAsync();
            return _mapper.Map<List<CategoryDto>>(categoryList);
        }

        public async Task<PaginatedResponseDto<CategoryDto>> GetCategoriesListAsync(PaginationRequestDto paginationRequest)
        {
            var currentUser = await _userService.GetCurrentUserAsync();
            if (currentUser == null)
                throw new UnauthorizedAccessException("User not authenticated.");
            var query = _dbContext.Categories.Where(category => category.IsDefault || category.CreatedBy == currentUser.Id);
            return await query.ProjectTo<CategoryDto>(_mapper.ConfigurationProvider)
                .ToPaginatedListAsync<CategoryDto>(paginationRequest.PageNumber, paginationRequest.PageSize);
        }

        public async Task<CategoryDto> GetCategoryByIdAsync(string Id)
        {
            var category = await _dbContext.Categories.FirstOrDefaultAsync(category => category.Id == Id);
            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<CategoryDto> CreateCategoryAsync(CategoryDto categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto);

            if (string.IsNullOrEmpty(category.Id))
            {
                category.Id = Guid.NewGuid().ToString();
            }

            var currentUser = await _userService.GetCurrentUserAsync();
            category.CreatedBy = currentUser.Id;
            category.CreatedAt = DateTime.UtcNow;

            var isAdmin = await _userService.IsCurrentUserAdminAsync();
            category.IsDefault = isAdmin;

            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<CategoryDto>(category);
        }


        public async Task<CategoryDto> UpdateCategoryAsync(string Id, CategoryDto categoryDto)
        {
            var category = await _dbContext.Categories.FirstOrDefaultAsync(category => category.Id == Id);
            _mapper.Map(categoryDto, category);
            if(category == null)
            {
                throw new KeyNotFoundException("Category not found");
            }
            else if (category.IsDefault && !await _userService.IsCurrentUserAdminAsync())
            {
                throw new InvalidOperationException("Cannot update default category");
            }
            var currentUser = await _userService.GetCurrentUserAsync();
            category.UpdatedBy = currentUser.Id;
            category.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            return _mapper.Map<CategoryDto>(category);
        }

        public async Task DeleteCategoryAsync(string Id)
        {
            var category = await _dbContext.Categories.FirstOrDefaultAsync(category => category.Id == Id);
            if (category == null)
            {
                throw new KeyNotFoundException("Category not found");
            }
            else if(category.IsDefault && !await _userService.IsCurrentUserAdminAsync())
            {
                throw new InvalidOperationException("Cannot delete default category");
            }
            _dbContext.Categories.Remove(category);
            await _dbContext.SaveChangesAsync();
        }

    }
}
