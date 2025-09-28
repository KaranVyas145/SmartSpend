using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartSpend.Dtos;
using SmartSpend.Helper;
using SmartSpend.Services;

namespace SmartSpend.Controllers
{
    [Authorize]
    [Route("category")]
    [ApiController]
    public class CategoryController
    {
        private ILogger<CategoryController> _logger;
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDto categoryDto)
        {
            try
            {
                var category = await _categoryService.CreateCategoryAsync(categoryDto);
                return ApiResponse.Success(category);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "An error occurred while creating a category.");
                return ApiResponse.BadRequest("An error occurred while creating a category.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                return ApiResponse.Success(categories);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "An error occurred while getting categories.");
                return ApiResponse.BadRequest("An error occurred while getting categories.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(string id)
        {
            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(id);
                return ApiResponse.Success(category);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "An error occurred while getting a category.");
                return ApiResponse.BadRequest("An error occurred while getting a category.");
            }
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetCategoryList([FromQuery] PaginationRequestDto paginationRequestDto)
        {
            try
            {
                var categories = await _categoryService.GetCategoriesListAsync(paginationRequestDto);
                return ApiResponse.Success(categories);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "An error occurred while getting category list.");
                return ApiResponse.BadRequest("An error occurred while getting category list.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(string id, [FromBody] CategoryDto categoryDto)
        {
            try
            {
                var category = await _categoryService.UpdateCategoryAsync(id, categoryDto);
                return ApiResponse.Success(category);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "An error occurred while updating a category.");
                return ApiResponse.BadRequest("An error occurred while updating a category.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(string id)
        {
            try
            {
                await _categoryService.DeleteCategoryAsync(id);
                return ApiResponse.Success("Category deleted successfully.");
            }
            catch (Exception ex) {
                _logger.LogError(ex, "An error occurred while deleting a category.");
                return ApiResponse.BadRequest("An error occurred while deleting a category.");
            }
        }
        


    }
}
