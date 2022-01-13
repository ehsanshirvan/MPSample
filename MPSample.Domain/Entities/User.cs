using System.Collections.Generic;

namespace MPSample.Domain.Entities
{
    public  class User : BaseEntity
    {
        
        public string  UserName { get; set; }
        public string Password { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
        public User()
        {
            Transactions = new HashSet<Transaction>();
        }
    }
}
