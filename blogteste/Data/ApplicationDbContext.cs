using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using blogteste.Models;

namespace blogteste.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<blogteste.Models.artigos> artigos { get; set; } = default!;
        public DbSet<blogteste.Models.ApplicationUser> ApplicationUser { get; set; } = default!;
    }
}
