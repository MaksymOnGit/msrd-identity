using FluentValidation;
using MailKit.Security;
using MSRD.Identity.Core.Validators;

namespace MSRD.Identity.Core
{
    public sealed class AppSettings
    {
        public AdminUser? AdminUser { get; init; }
        public SmtpSettings? SmtpSettings { get; set; }
        public Templates? Templates { get; set; }
        public string? BaseAddress { get; set; }
    }

    public class AdminUser
    {
        public string? Email { get; set; }
        public string? Password { set; get; }
    }
    public sealed class SmtpSettings
    {
        public string? SmtpLogin { get; set; }
        public string? SmtpPassword { get; set; }
        public string? SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public string? SmtpSecureSocketOption { get; set; }
        public string? SmtpSenderAddress { get; set; }
        public string? SmtpSenderName { get; set; }
    }

    public sealed class Templates
    {
        public string? ConfirmEmailEmailTemplatePath { get; set; } = "HtmlTemplates\\ConfirmEmailEmailTemplate.html";
        public string? ConfirmEmailUriTemplate { get; set; }
    }

    public sealed class AppSettingsValidator : AbstractValidator<AppSettings>
    {
        public AppSettingsValidator()
        {

            RuleFor(x => x.SmtpSettings).NotNull().DependentRules(() =>
            {
                RuleFor(x => x.SmtpSettings!.SmtpHost).NotEmpty();
                RuleFor(x => x.SmtpSettings!.SmtpLogin).NotEmpty();
                RuleFor(x => x.SmtpSettings!.SmtpPassword).NotEmpty();
                RuleFor(x => x.SmtpSettings!.SmtpPort).NotEmpty();
                RuleFor(x => x.SmtpSettings!.SmtpSecureSocketOption).NotEmpty().IsEnumName(typeof(SecureSocketOptions));
                RuleFor(x => x.SmtpSettings!.SmtpSenderAddress).NotEmpty().EmailAddress();
                RuleFor(x => x.SmtpSettings!.SmtpSenderName).NotEmpty();
            });

            RuleFor(x => x.Templates).NotNull().DependentRules(() =>
            {
                RuleFor(x => x.Templates!.ConfirmEmailEmailTemplatePath).NotEmpty().MustBeValidFilePath();

                RuleFor(x => x.Templates!.ConfirmEmailUriTemplate).Must(url => url.Contains("{0}") && url.Contains("{1}")).WithMessage("{PropertyName} must contain {0} for email and {1} placeholder for token.").Must(uri =>
                {
                    Uri uriResult;
                    return Uri.TryCreate(String.Format(uri, "e@m.ail", "token"), UriKind.Absolute, out uriResult)
                        && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                }).WithMessage("{PropertyName} must be a valid http or https url formatted string.");
            });
        }
    }
}