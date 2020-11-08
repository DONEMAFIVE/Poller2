using Microsoft.EntityFrameworkCore;

namespace PollerConsoleApp
{
    public class dbService : DbContext
    {    
        public DbSet<table_csharp_test> csharp_test { get; set; }
        
        public dbService()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql("server=localhost;UserId=root;Password=;database=csharp_test");
            //optionsBuilder.UseMySql("server=localhost;user=root;database=csharp_test;SslMode=none;password=");
        }
    }
}