using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MPSample.Domain;
using MPSample.Domain.Dto;
using MPSample.Domain.Entities;
using MPSample.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MPSample.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransController : ControllerBase
    {
       

        private readonly ILogger<TransController> _logger;
        
        public TransController(ILogger<TransController> logger)
        {
            _logger = logger;
            
        }

        [HttpPost]
        public IActionResult SaveTransaction([FromServices]TransactionService srvc,[FromBody] TransactionDto model)
        {
            var reslt = srvc.Save(model);
            if (reslt.IsSuccess)
                return Ok();
            return BadRequest(reslt.ErrorMessage);
        }

        
        [HttpGet]
        public IActionResult GetAllTransaction([FromServices] TransactionService srvc)
        {
            var result = srvc.GetAll();
            return new JsonResult(result);
        }

        
        [HttpGet("{merchant}")]
        public IActionResult GetByMerchantName([FromServices] TransactionService srvc,string merchant)
        {
            var result = srvc.GetByMerchantName(merchant);
            return new JsonResult(result);
        }

        [Route("[action]/{merchant}")]
        public IActionResult GetFee([FromServices] TransactionService srvc, string merchant)
        {
            var result = srvc.GetFeeByMerchantName(merchant);
            return new JsonResult(result);
        }

    }
}
