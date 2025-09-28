using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using SmartSpend.Data;
using SmartSpend.Dtos;
using SmartSpend.Extensions;
using SmartSpend.Models;

namespace SmartSpend.Services
{
    public interface ITransactionService
    {
        public Task<List<TransactionDto>> GetAllTransactionsAsync();
        public Task<PaginatedResponseDto<TransactionDto>> GetTransactionsListAsync(PaginationRequestDto paginationRequestDto);
        public Task<TransactionDto> GetTransactionByIdAsync(string Id);
        public Task<TransactionDto> CreateTransactionAsync(TransactionDto transactionDto);
        public Task<TransactionDto> UpdateTransactionAsync(string Id, TransactionDto transactionDto);
        public Task DeleteTransactionAsync(string Id);
    }

    public class TransactionService : ITransactionService
    {
        public readonly ILogger<TransactionService> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public TransactionService(ILogger<TransactionService> logger, ApplicationDbContext dbContext, IMapper mapper, IUserService userService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<List<TransactionDto>> GetAllTransactionsAsync()
        {
            var transactionList = await _dbContext.Transactions.ToListAsync();
            return _mapper.Map<List<TransactionDto>>(transactionList);
        }

        public async Task<PaginatedResponseDto<TransactionDto>> GetTransactionsListAsync(PaginationRequestDto paginationRequest)
        {
            var currentUser = await _userService.GetCurrentUserAsync();
            if (currentUser == null)
                throw new UnauthorizedAccessException("User not authenticated.");
            var query = _dbContext.Transactions.AsNoTracking();
            return await query.ProjectTo<TransactionDto>(_mapper.ConfigurationProvider)
                .ToPaginatedListAsync<TransactionDto>(paginationRequest.PageNumber, paginationRequest.PageSize);
        }

        public async Task<TransactionDto> GetTransactionByIdAsync(string Id)
        {
            var transaction = await _dbContext.Transactions.FirstOrDefaultAsync(transaction => transaction.Id == Id);
            return _mapper.Map<TransactionDto>(transaction);
        }

        public async Task<TransactionDto> CreateTransactionAsync(TransactionDto transactionDto)
        {
            var transaction = _mapper.Map<Transaction>(transactionDto);

            if (string.IsNullOrEmpty(transaction.Id))
            {
                transaction.Id = Guid.NewGuid().ToString();
            }

            var currentUser = await _userService.GetCurrentUserAsync();
            transaction.CreatedBy = currentUser.Id;
            transaction.CreatedAt = DateTime.UtcNow;

            var isAdmin = await _userService.IsCurrentUserAdminAsync();

            await _dbContext.Transactions.AddAsync(transaction);
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<TransactionDto>(transaction);
        }


        public async Task<TransactionDto> UpdateTransactionAsync(string Id, TransactionDto transactionDto)
        {
            var transaction = await _dbContext.Transactions.FirstOrDefaultAsync(transaction => transaction.Id == Id);
            _mapper.Map(transactionDto, transaction);
            if (transaction == null)
            {
                throw new KeyNotFoundException("Transaction not found");
            }
            var currentUser = await _userService.GetCurrentUserAsync();
            transaction.UpdatedBy = currentUser.Id;
            transaction.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            return _mapper.Map<TransactionDto>(transaction);
        }

        public async Task DeleteTransactionAsync(string Id)
        {
            var transaction = await _dbContext.Transactions.FirstOrDefaultAsync(transaction => transaction.Id == Id);
            if (transaction == null)
            {
                throw new KeyNotFoundException("Transaction not found");
            }
            _dbContext.Transactions.Remove(transaction);
            await _dbContext.SaveChangesAsync();
        }

    }
}
