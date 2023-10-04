using Common.Entities;
using Microsoft.EntityFrameworkCore;

namespace Common.Context
{
    public class AppContext : DbContext
    {
        public AppContext(DbContextOptions<AppContext> options) : base(options)
        {}

        public DbSet<User> User { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer("Server=.\\pooyesh;Database=Auth;TrustServerCertificate=True;Trusted_Connection=True;");
        }
    }
}
