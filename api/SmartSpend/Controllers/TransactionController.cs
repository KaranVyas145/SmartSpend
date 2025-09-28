using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartSpend.Dtos;
using SmartSpend.Helper;
using SmartSpend.Services;

namespace SmartSpend.Controllers
{
    [Authorize]
    [Route("transaction")]
    [ApiController]
    public class TransactionController
    {
        private ILogger<TransactionController> _logger;
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService, ILogger<TransactionController> logger)
        {
            _transactionService = transactionService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTransaction([FromBody] TransactionDto transactionDto)
        {
            try
            {
                var transaction = await _transactionService.CreateTransactionAsync(transactionDto);
                return ApiResponse.Success(transaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a transaction.");
                return ApiResponse.BadRequest("An error occurred while creating a transaction.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTransactions()
        {
            try
            {
                var transactions = await _transactionService.GetAllTransactionsAsync();
                return ApiResponse.Success(transactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting transactions.");
                return ApiResponse.BadRequest("An error occurred while getting transactions.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransactionById(string id)
        {
            try
            {
                var transaction = await _transactionService.GetTransactionByIdAsync(id);
                return ApiResponse.Success(transaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting a transaction.");
                return ApiResponse.BadRequest("An error occurred while getting a transaction.");
            }
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetTransactionList([FromQuery] PaginationRequestDto paginationRequestDto)
        {
            try
            {
                var transactions = await _transactionService.GetTransactionsListAsync(paginationRequestDto);
                return ApiResponse.Success(transactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting transaction list.");
                return ApiResponse.BadRequest("An error occurred while getting transaction list.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransaction(string id, [FromBody] TransactionDto transactionDto)
        {
            try
            {
                var transaction = await _transactionService.UpdateTransactionAsync(id, transactionDto);
                return ApiResponse.Success(transaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating a transaction.");
                return ApiResponse.BadRequest("An error occurred while updating a transaction.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(string id)
        {
            try
            {
                await _transactionService.DeleteTransactionAsync(id);
                return ApiResponse.Success("Transaction deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting a transaction.");
                return ApiResponse.BadRequest("An error occurred while deleting a transaction.");
            }
        }



    }
}
