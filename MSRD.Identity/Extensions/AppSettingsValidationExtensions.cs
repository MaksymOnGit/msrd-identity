using FluentValidation;
using Microsoft.Extensions.Options;

namespace MSRD.Identity.Extensions
{
    public static class AppSettingsValidationExtensions
    {
        public static IApplicationBuilder ValidateSettings<TSettingsValidator, TAppSettings>(this IApplicationBuilder app) where TSettingsValidator : AbstractValidator<TAppSettings>, new() where TAppSettings : class
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var services = scope.ServiceProvider;
                var options = services.GetService<IOptions<TAppSettings>>()?.Value;
                var validator = new TSettingsValidator();
                var result = validator.Validate(options);

                if (!result.IsValid)
                {
                    var logger = services.GetService<ILogger<TAppSettings>>();
                    logger.LogError(result.ToString());

                    throw new Exception(result.ToString());
                }
            }

            return app;
        }
    }
}
