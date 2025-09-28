using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartSpend.Models;

namespace SmartSpend.Data
{
    public class ApplicationDbContext: IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
        public DbSet<Category> Categories { get; set; } 
    }
}
