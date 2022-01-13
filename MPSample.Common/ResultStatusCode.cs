using System.ComponentModel.DataAnnotations;

namespace MPSample.Common
{
    public enum ResultStatusCode
    {
        [Display(Name = "UnAuthorized Request")]
        UnAuthorized = 1,

        [Display(Name = "BadRequest Has Been Sent")]
        BadRequest = 2,

        [Display(Name = "NotFound")]
        NotFound = 3,

        [Display(Name = "LogicError Occured")]
        LogicError = 4,

        [Display(Name = "Unknown Server Error Occured")]
        ServerError = 5,

        [Display(Name = "Request Succeeded")]
        Success = 6
    }
}
