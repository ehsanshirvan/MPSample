using MPSample.Common;
using MPSample.Domain.Common;
using MPSample.Domain.Dto;
using MPSample.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPSample.Domain.Services
{
    public class TransactionService
    {

        #region Fields
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Transaction> _repository;
        #endregion

        #region Public Methods
        public TransactionService(IUnitOfWork unitOfWork, IRepository<Transaction> repo)
        {
            this._unitOfWork = unitOfWork;
            _repository = repo;
        }
        public ServiceResult Save(TransactionDto userDto)
        {
            var item = Mapper.Map(userDto);

            _repository.Add(item);

            _unitOfWork.Commit();
            return new ServiceResult(ResultStatusCode.Success, true);


        }

        public ServiceResult Save(List<TransactionDto> userDtoList)
        {
            var listToAdd = Mapper.Map(userDtoList);
            _repository.AddRange(listToAdd);
            _unitOfWork.Commit();
            return new ServiceResult(ResultStatusCode.Success, true);


        }

        public ServiceResult<List<TransactionDto>> GetAll()
        {
            var listData = _repository.GetAll().ToList();
            var resultData = Mapper.Map(listData);
            var result = new ServiceResult<List<TransactionDto>>(ResultStatusCode.Success,result:resultData);
            
            return result;
        }

        public ServiceResult<List<TransactionDto>> GetByMerchantName(string merchantName)
        {
            var listData = _repository.GetBy(x => x.MerchantName.ToLower() == merchantName.ToLower()).ToList();
            var resultData = Mapper.Map(listData);
            var result = new ServiceResult<List<TransactionDto>>(ResultStatusCode.Success, result: resultData);

            return result;
        }


        
        public ServiceResult<double> GetFeeByMerchantName(string merchantName)
        {
            double finalResult= 0;
            var listData = _repository.GetBy(x => x.MerchantName.ToLower() == merchantName.ToLower()).ToList();

            //all the merchants must be charged of 1% of all transactions in which case they would'nt be done in weekend
            var finalAmountToPay = listData
                .Where(x => x.TimeStamp.DayOfWeek != DayOfWeek.Saturday && x.TimeStamp.DayOfWeek != DayOfWeek.Sunday)
                .Sum(x => x.Amount) * .001;

            //if all transactions made during weekend then the fee is free
            if (finalAmountToPay > 0)
            {

                var finalBaseAmountToPay = finalAmountToPay;
                //calculate discount based on merchantName
                var discountPerMerchant = _getDiscountBasedOnMerchant(merchantName, finalAmountToPay);

                //calculate extra discount based on transactions count on each month
                var extraDiscount = _getExtraDiscount(listData);

                finalResult = finalAmountToPay - discountPerMerchant - extraDiscount;
            }
            var result = new ServiceResult<double>(ResultStatusCode.Success,result: finalResult);

            return result;
        }
        #endregion

        #region Private Methods
        private double _getExtraDiscount(List<Transaction> listData)
        {
            long tmpResult = 0;
            double result = 0;
            var tmp = listData.GroupBy(x => new { x.TimeStamp.Year, x.TimeStamp.Month }).ToList();
            foreach (var group in tmp)
            {
                var subList = group.ToList();
                if (subList.Count > 10) {
                    tmpResult += subList.Sum(x => x.Amount);
                };
            }
            result = tmpResult * .25;
            return result;
        }

        private double _getDiscountBasedOnMerchant(string merchantName, double finalAmountToPay)
        {
            switch (merchantName)
            {
                case "tesla":
                    finalAmountToPay -=  finalAmountToPay * .25;
                    break;

                case "mcdonald":
                    finalAmountToPay -= finalAmountToPay * .05;
                    break;

                case "rema1000":
                    finalAmountToPay -= finalAmountToPay * .15;
                    break;

                default:
                    finalAmountToPay  = 0;
                    break;
            }

            return finalAmountToPay;
        }
        #endregion
    }
}
