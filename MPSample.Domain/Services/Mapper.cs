using MPSample.Domain.Dto;
using MPSample.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPSample.Domain.Services
{
    public class Mapper
    {
        public static Transaction Map(TransactionDto model)
        {
            return new Transaction
            {
                Amount = model.Amount,
                MerchantName = model.MerchantName,
                TimeStamp = model.TimeStamp
            };
        }

        public static List<Transaction> Map(List<TransactionDto> model)
        {
            return model.Select(x => new Transaction {
                Amount = x.Amount,
                MerchantName = x.MerchantName,
                TimeStamp = x.TimeStamp
            }).ToList();
        }

        public static TransactionDto Map(Transaction model)
        {
            return new TransactionDto
            {
                Amount = model.Amount,
                MerchantName = model.MerchantName,
                TimeStamp = model.TimeStamp
            };
        }

        public static List<TransactionDto> Map(List<Transaction> model)
        {
            return model.Select(x => new TransactionDto
            {
                Amount = x.Amount,
                MerchantName = x.MerchantName,
                TimeStamp = x.TimeStamp
            }).ToList();
        }

    }
}
