using Microsoft.EntityFrameworkCore;
using MPSample.Domain.Entities;
using MPSample.Infrastructure.DataAccess;

namespace MPSample.Infrastructure.Repositories
{
    public class TransactionRepository : RepositoryBase<Transaction>
    {
        private readonly MPDbContext dbContext;

        public TransactionRepository(MPDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
    }
}
