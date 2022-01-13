using System;
using System.Collections.Generic;
using System.Text;

namespace MPSample.Domain.Dto
{
    public class TransactionDto
    {
        public string MerchantName { get; set; }
        public long Amount { get; set; }
        
        
        public DateTime TimeStamp { get; set; }
        
    }
}
