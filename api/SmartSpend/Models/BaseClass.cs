using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SmartSpend.Models
{
    public class BaseClass
    {
        [Key]
        public string Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        public string? EnabledBy { get; set; }
        public bool Enabled { get; set; }

        public DateTime? LastEnabledDisabled { get; set; }

        [Timestamp]
        [Column(TypeName = "bytea")]
        public byte[]? RowVersion { get; set; }

        public BaseClass()
        {
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            Id = Guid.NewGuid().ToString();
        }
    }

}
