using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MPSample.Domain;
using MPSample.Infrastructure.DataAccess;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MPSample.Infrastructure.Repositories
{

    public class UnitOfWork : IUnitOfWork
    {
        private MPDbContext _dbContext;
        private UserRepository _users;
        private TransactionRepository _transactions;
        #region Properties



        public UserRepository Users
        {
            get
            {
                if (_users == null)
                {
                    _users = new UserRepository(_dbContext);
                }
                return _users;
            }
        }

        public TransactionRepository Trancactions
        {
            get
            {
                if (_transactions == null)
                {
                    _transactions = new TransactionRepository(_dbContext);
                }
                return _transactions;
            }
        }

        #endregion


        #region Public Methods
        public UnitOfWork(MPDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public int Commit()
        {
            var entityForSave = GetEntityForSave();
            if (entityForSave != null && entityForSave.Count > 0)
                return _dbContext.SaveChanges();
            return 0;
        }

        #endregion

        #region Private Methods

        #endregion

        private List<EntityEntry> GetEntityForSave()
        {
            return _dbContext.ChangeTracker
              .Entries()
              .Where(x => x.State == EntityState.Modified || x.State == EntityState.Added || x.State == EntityState.Deleted)
              .ToList();
        }

        public Task SaveChangesAsync()
        {
            return _dbContext.SaveChangesAsync();
        }

       
    }
    
}
