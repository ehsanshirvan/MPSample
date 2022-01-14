using Microsoft.EntityFrameworkCore;
using MPSample.Domain.Entities;

namespace MPSample.Infrastructure.DataAccess
{
    public class MPDbContext : DbContext
    {


        public MPDbContext(DbContextOptions<MPDbContext> options) : base(options)
        {


            //Database.Migrate();
        }

        public MPDbContext()
        {
        }


        bool? _inMemoryMode;
        public MPDbContext(bool inMemoryMode)
        {
            _inMemoryMode = inMemoryMode;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(_inMemoryMode.HasValue && _inMemoryMode.Value)
                optionsBuilder.UseInMemoryDatabase("MPSampleDb");

            else
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlServer("Server=.\\sqlexpress;Database=MPSampleDb;Integrated Security=True;Trusted_Connection=True;");
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Transaction> Transactions { get; set; }


       

    }

   
}
