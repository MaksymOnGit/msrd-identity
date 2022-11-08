using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MSRD.Identity.BusinessLogic.Services.Interfaces;
using MSRD.Identity.Extensions;
using MSRD.Identity.Models.Account.Requests;
using MSRD.Identity.PersistentStorage.Repositories;

namespace MSRD.Identity.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public sealed class AccountController : ControllerBase
    {
        private readonly IAccountService accountService;

        public AccountController(IAccountService accountService)
        {
            this.accountService = accountService ?? throw new ArgumentNullException(nameof(IdentityRepository));
        }

        [HttpPost]
        public async ValueTask<IActionResult> Registration([FromServices]IValidator<RegistrationRequest> requestValidator, RegistrationRequest registrationRequest, CancellationToken cancellationToken = default)
        {
            var validationResult = requestValidator.Validate(registrationRequest);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.ToErrorResponse());

            var result = await accountService.RegisterNewUserAsync(registrationRequest.Email, registrationRequest.InvitationToken, registrationRequest.Password, cancellationToken);

            if (result.IsFailed)
                return BadRequest(result.ToErrorResponse());

            return Ok();
        }
    }
}
