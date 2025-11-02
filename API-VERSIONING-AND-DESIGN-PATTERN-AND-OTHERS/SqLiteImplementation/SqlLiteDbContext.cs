using Microsoft.EntityFrameworkCore;

namespace SqLiteImplementation
{
    public class SqlLiteDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("Data Source=product.db;");

        public DbSet<ProductModel> productModels { get; set; }
        }
}
