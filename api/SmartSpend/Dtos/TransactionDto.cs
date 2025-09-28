using SmartSpend.Enums;

namespace SmartSpend.Dtos
{
    public class TransactionDto
    {
        public string? UserId { get; set; }
        public string? CategoryId { get; set; }
        public EnumTransactionType TransactionType { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public CategoryDto? Category { get; set; }

    }
}
