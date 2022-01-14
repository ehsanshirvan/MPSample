using Microsoft.Extensions.Configuration;
using MPSample.Domain.Dto;
using MPSample.Domain.Services;
using MPSample.Infrastructure.DataAccess;
using MPSample.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace MPSample.Test
{
    public class TransactionServiceTest
    {
        MPDbContext dbContext;
        public TransactionServiceTest()
        {
            dbContext = GetDbContext();
        }


        #region Public Methods
        [Fact]
        public void Save_Return_True()
        {
            //Arange
            
            var transService = new TransactionService(new UnitOfWork(dbContext) ,new TransactionRepository(dbContext));
            
            //Act
            var saveResult = transService.Save(new Domain.Dto.TransactionDto {Amount = 10,MerchantName = "tesla",TimeStamp = DateTime.Now });
           
            //Assert
            Assert.True(saveResult.IsSuccess);

        }

        [Fact]
        public void Save_List_Return_True()
        {
            //Arange           
            var transService = new TransactionService(new UnitOfWork(dbContext), new TransactionRepository(dbContext));

            //Act
            var itemsToSave = new List<TransactionDto>();
            _loadData(itemsToSave);
            var saveResult = transService.Save(itemsToSave);

            //Assert
            Assert.True(saveResult.IsSuccess);

        }

        [Fact]
        public void Get_Transactions_By_Merchant_Must_Not_To_Have_OtherMerchnts()
        {
            //Arange            
            var transService = new TransactionService(new UnitOfWork(dbContext), new TransactionRepository(dbContext));

            //Act
            var merchantName = "tesla";
            var queryResult = transService.GetByMerchantName(merchantName);

            //Assert
            Assert.True(!queryResult.Result.Any(x=>x.MerchantName.ToLower() != merchantName));
        }

        [Fact]
        public void Get_Transactions_By_Merchant_Must_Not_To_Have_Any_Transaction_With_ZeroAmount()
        {
            //Arange
            var transService = new TransactionService(new UnitOfWork(dbContext), new TransactionRepository(dbContext));

            //Act
            var merchantName = "tesla";
            var queryResult = transService.GetByMerchantName(merchantName);

            //Assert
            Assert.True(!queryResult.Result.Any(x => x.Amount == 0));
        }

        [Fact]
        public void GetAll_Must_Have_AtLeast_One()
        {
            //Arange
            var transService = new TransactionService(new UnitOfWork(dbContext), new TransactionRepository(dbContext));

            //Act            
            var queryResult = transService.GetAll();

            //Assert
            Assert.True(queryResult.Result.Any());
        }

        [Fact]
        public void GetAll_Must_Not_Be_Null()
        {
            //Arange
            var transService = new TransactionService(new UnitOfWork(dbContext), new TransactionRepository(dbContext));

            //Act            
            var queryResult = transService.GetAll();

            //Assert
            Assert.True(queryResult.Result != null);
        }

        [Fact]
        public void GetFee_ByMerchant_Must_Be_GreaterEqual_Zero()
        {
            //Arange
            var transService = new TransactionService(new UnitOfWork(dbContext), new TransactionRepository(dbContext));

            //Act            
            var merchant = "tesla";
            var queryResult = transService.GetFeeByMerchantName(merchant);

            //Assert
            Assert.True(queryResult.Result >= 0);
        }
        
        [Fact]
        public void Get_Transactions_By_Merchant_Must_Not_To_Have_OtherMerchnts_Then_IsSuccess()
        {
            //Arange
            var transService = new TransactionService(new UnitOfWork(dbContext), new TransactionRepository(dbContext));

            //Act
            var merchantName = "tesla";
            var queryResult = transService.GetByMerchantName(merchantName);

            //Assert
            Assert.True(!queryResult.Result.Any(x => x.MerchantName.ToLower() != merchantName) && queryResult.IsSuccess);
        }

        [Fact]
        public void Get_Transactions_By_Merchant_Must_Not_To_Have_Any_Transaction_With_ZeroAmount_Then_IsSuccess()
        {
            //Arange
            var transService = new TransactionService(new UnitOfWork(dbContext), new TransactionRepository(dbContext));

            //Act
            var merchantName = "tesla";
            var queryResult = transService.GetByMerchantName(merchantName);

            //Assert
            Assert.True(!queryResult.Result.Any(x => x.Amount == 0) && queryResult.IsSuccess);
        }

        [Fact]
        public void GetAll_Must_Have_AtLeast_One_Then_IsSuccess()
        {
            //Arange
            var transService = new TransactionService(new UnitOfWork(dbContext), new TransactionRepository(dbContext));

            //Act            
            var queryResult = transService.GetAll();

            //Assert
            Assert.True(queryResult.Result.Any() && queryResult.IsSuccess);
        }

        [Fact]
        public void GetAll_Must_Not_Be_Null_Then_IsSuccess()
        {
            //Arange
            var transService = new TransactionService(new UnitOfWork(dbContext), new TransactionRepository(dbContext));

            //Act            
            var queryResult = transService.GetAll();

            //Assert
            Assert.True(queryResult.Result != null && queryResult.IsSuccess);
        }

         [Fact]
        public void GetFee_ByMerchant_Must_Not_Return_Not_DoubleValue()
        {
            //Arange
            var transService = new TransactionService(new UnitOfWork(dbContext), new TransactionRepository(dbContext));

            //Act            
            var merchant = "tesla";
            var queryResult = transService.GetFeeByMerchantName(merchant);

            //Assert
            double retVal = 0;
            Assert.True(double.TryParse(queryResult.Result.ToString(),out retVal));
        }


        #endregion

        #region Private Methods


        private void _loadData(List<TransactionDto> itemsToSave)
        {
            itemsToSave.Add(new TransactionDto { Amount = 100000, MerchantName = "tesla", TimeStamp = DateTime.Now });
            itemsToSave.Add(new TransactionDto { Amount = 2000000, MerchantName = "tesla", TimeStamp = DateTime.Now.AddDays(-10) });
            itemsToSave.Add(new TransactionDto { Amount = 2000000, MerchantName = "tesla", TimeStamp = DateTime.Now.AddDays(-30) });
            itemsToSave.Add(new TransactionDto { Amount = 500000, MerchantName = "tesla", TimeStamp = DateTime.Now.AddDays(-24) });
            itemsToSave.Add(new TransactionDto { Amount = 2000000, MerchantName = "tesla", TimeStamp = DateTime.Now.AddDays(-4) });
            itemsToSave.Add(new TransactionDto { Amount = 30000000, MerchantName = "tesla", TimeStamp = DateTime.Now.AddDays(-2) });
        }



        private MPDbContext GetDbContext()
        {

            var config = new ConfigurationBuilder()
                            .AddJsonFile("TestAppSettings.json")
                            .Build();


            var isInmemory = bool.Parse(config.GetSection("InMemoryMode").Value.ToString());

            return new MPDbContext(isInmemory);
        }

        #endregion
    }
}
