using SmartSpend.Enums;

namespace SmartSpend.Models
{
    public class Category : BaseClass
    {
        public string Name { get; set; }
        public EnumTransactionType TransactionType { get; set; }
        public bool IsDefault { get; set; }
    }
}
