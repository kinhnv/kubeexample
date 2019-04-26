using Microsoft.EntityFrameworkCore;

namespace Service1.Datas {
    public class ApplicationDbContext : DbContext {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            :base(options)
        {            
        }

        public DbSet<Product> Products { get; set; }
    }
}