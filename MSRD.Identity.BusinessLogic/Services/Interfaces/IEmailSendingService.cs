using FluentResults;

namespace MSRD.Identity.BusinessLogic.Services.Interfaces
{
    public interface IEmailSendingService
    {
        ValueTask<Result> SendRegistrationConfirmationEmailAsync(string to, string confirmationToken, CancellationToken cancellationToken = default);
    }
}
