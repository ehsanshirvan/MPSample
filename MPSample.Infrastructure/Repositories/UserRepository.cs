using Microsoft.EntityFrameworkCore;
using MPSample.Domain.Entities;
using MPSample.Infrastructure.DataAccess;

namespace MPSample.Infrastructure.Repositories
{
    public class UserRepository : RepositoryBase<User>
    {
        private readonly MPDbContext dbContext;

        public UserRepository(MPDbContext dbContext):base(dbContext)
        {
            this.dbContext = dbContext;
        }
    }
}
