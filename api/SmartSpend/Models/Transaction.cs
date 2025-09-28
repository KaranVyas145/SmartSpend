using SmartSpend.Enums;

namespace SmartSpend.Models
{
    public class Transaction: BaseClass
    {
        public string UserId { get; set; }
        public string CategoryId { get; set; }
        public EnumTransactionType TransactionType { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime TransactionDate { get; set; }
        public virtual Category Category { get; set; }
        public virtual User User { get; set; }
        
    }
}
