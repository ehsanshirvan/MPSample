using Microsoft.EntityFrameworkCore;
using MPSample.Domain.Entities;

namespace MPSample.Infrastructure.DataAccess
{
    public class MPDbContext : DbContext
    {
        public MPDbContext(DbContextOptions<MPDbContext> options) : base(options)
        {
        }

        public MPDbContext()
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
    }
}
