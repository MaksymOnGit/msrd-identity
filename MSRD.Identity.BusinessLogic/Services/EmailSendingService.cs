using FluentResults;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MSRD.Identity.BusinessLogic.Results.EmailSending;
using MSRD.Identity.BusinessLogic.Services.Interfaces;
using MSRD.Identity.Core;
using System.Text;

namespace MSRD.Identity.BusinessLogic.Services
{
    public sealed class EmailSendingService : IEmailSendingService
    {
        private readonly SmtpSettings smtpSettings;
        private readonly SecureSocketOptions secureSocketOption;
        private readonly Templates templates;
        public EmailSendingService(IOptionsSnapshot<SmtpSettings> smtpSettings, IOptionsSnapshot<Templates> templates)
        {
            this.smtpSettings = smtpSettings.Value;
            this.templates = templates.Value;
            secureSocketOption = Enum.Parse<SecureSocketOptions>(this.smtpSettings.SmtpSecureSocketOption);
        }

        private async ValueTask<string> ReadTemplateAsync(string path)
        {
            using StreamReader sr = new StreamReader(path, Encoding.UTF8);
            return await sr.ReadToEndAsync();
        }

        public async ValueTask<Result> SendRegistrationConfirmationEmailAsync(string to, string confirmationToken, CancellationToken cancellationToken = default)
        {
            var template = await ReadTemplateAsync(templates.ConfirmEmailEmailTemplatePath);
            var confirmationUriTemplate = templates.ConfirmEmailUriTemplate;
            confirmationUriTemplate = String.Format(confirmationUriTemplate, to, confirmationToken);
            template = template.Replace("{{confirmationUrl}}", confirmationUriTemplate);
            return await SendEmailAsync(to, "Please confirm your MSRD account", template, cancellationToken);
        }

        private async ValueTask<Result> SendEmailAsync(string to, string subject, string messageText, CancellationToken cancellationToken = default)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(smtpSettings.SmtpSenderName, smtpSettings.SmtpSenderAddress));
            message.To.Add(new MailboxAddress(to, to));
            message.Subject = subject;
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = messageText;
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            try
            {
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                await client.ConnectAsync(smtpSettings.SmtpHost, smtpSettings.SmtpPort, secureSocketOption, cancellationToken);
                await client.AuthenticateAsync(smtpSettings.SmtpLogin, smtpSettings.SmtpPassword, cancellationToken);

                await client.SendAsync(message, cancellationToken);
            }
            catch (Exception ex)
            {
                return Result.Fail(new ConfirmationEmailSendingError(to).CausedBy(ex)).Log<EmailSendingService>("Error while calling SendEmailAsync method", Microsoft.Extensions.Logging.LogLevel.Error);
            }
            finally
            {
                await client.DisconnectAsync(true, cancellationToken);
            }
            return Result.Ok();
        }
    }
}
