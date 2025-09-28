using SmartSpend.Enums;

namespace SmartSpend.Dtos
{
    public class CategoryDto
    {
        public string? Id { get; set; } // null for create, set for update/read
        public string Name { get; set; }
        public EnumTransactionType TransactionType { get; set; } // Income or Expense
        public bool IsDefault { get; set; }
    }

}
