using System;

namespace MPSample.Domain.Entities
{
    public class Transaction :BaseEntity
    {
        
        public string MerchantName { get; set; }
        public long Amount { get; set; }
        public DateTime TimeStamp { get; set; }
        public User User { get; set; }

    }
}
