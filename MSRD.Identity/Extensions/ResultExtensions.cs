using FluentResults;
using FluentValidation.Results;
using MSRD.Identity.Models;

namespace MSRD.Identity.Extensions
{
    public static class ResultExtensions
    {
        public static IEnumerable<ErrorResultResponseModel> ToErrorResponse(this Result result)
        {
            return result.Errors.Select(x => new ErrorResultResponseModel { Message = x.Message, Code = TransformErrorCode(x) });
        }
        public static IEnumerable<ErrorResultResponseModel> ToErrorResponse(this ValidationResult result)
        {
            return result.Errors.Select(x => new ErrorResultResponseModel { Message = x.ErrorMessage, Code = x.ErrorCode });
        }
        private static string TransformErrorCode(IError error)
        {
            if (error.Metadata.TryGetValue("ErrorCode", out var errorCode))
                return errorCode as string;

            return "";
        }
    }
}
