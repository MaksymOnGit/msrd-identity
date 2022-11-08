using FluentValidation;

namespace MSRD.Identity.Models.Account.Requests
{
    public sealed class RegistrationRequest
    {
        public string InvitationToken { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordRepeat { get; set; }
    }

    public sealed class RegistrationRequestValidator : AbstractValidator<RegistrationRequest>
    {
        public RegistrationRequestValidator()
        {
            RuleFor(request => request.Email).NotEmpty().EmailAddress();
            RuleFor(request => request.Password).NotEmpty();
            RuleFor(request => request.PasswordRepeat).NotEmpty().Equal(request => request.Password).WithMessage("Confirmation password field must match the password field.").WithErrorCode("InvalidConfirmationPassword");
            RuleFor(request => request.InvitationToken).NotEmpty();
        }
    }
}
