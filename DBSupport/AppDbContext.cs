using Microsoft.EntityFrameworkCore;
namespace vnenterprises.DBSupport
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}
