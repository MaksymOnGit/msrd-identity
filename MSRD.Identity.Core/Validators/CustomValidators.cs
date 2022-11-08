using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSRD.Identity.Core.Validators
{
    public static class CustomValidators
    {
        public static IRuleBuilderOptions<T, string> MustBeUriFormat<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.Must(uri =>
            {
                Uri uriResult;
                return Uri.TryCreate(uri, UriKind.Absolute, out uriResult)
                    && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            }).WithMessage("{PropertyName} must be a valid http or https url formatted string.");
        }
        public static IRuleBuilderOptions<T, string> MustBeValidFilePath<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.Must(path => System.IO.File.Exists(path)).WithMessage("{PropertyName} must be a valid file path. Value: {PropertyValue}");
        }
    }
}
