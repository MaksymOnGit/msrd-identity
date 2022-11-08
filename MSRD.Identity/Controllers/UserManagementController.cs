using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MSRD.Identity.BusinessLogic.Services;
using MSRD.Identity.BusinessLogic.Services.Interfaces;
using MSRD.Identity.Core.Query;
using MSRD.Identity.Extensions;
using MSRD.Identity.Models.UserManagement.Responses;
using MSRD.Identity.PersistentStorage.Repositories;
using OpenIddict.Server.AspNetCore;
using OpenIddict.Validation.AspNetCore;

namespace MSRD.Identity.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme, Roles = "Admin")]
    public class UserManagementController : ControllerBase
    {
        private readonly IAccountService accountService;
        private readonly IMapper mapper;

        public UserManagementController(IAccountService accountService, IMapper mapper)
        {
            this.accountService = accountService ?? throw new ArgumentNullException(nameof(IdentityRepository));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(IdentityRepository));
        }

        [HttpPost]
        public async ValueTask<IActionResult> InviteNewUser(string email, CancellationToken cancellationToken = default)
        {
            var result = await accountService.InviteNewUserAsync(email, cancellationToken);

            if (result.IsFailed)
                return BadRequest(result.ToErrorResponse());

            return Ok();
        }



        [HttpPost]
        public async ValueTask<IActionResult> ResendInvitation(string email, CancellationToken cancellationToken = default)
        {
            var result = await accountService.ResendInvitationEmailAsync(email, cancellationToken);

            if (result.IsFailed)
                return BadRequest(result.ToErrorResponse());

            return Ok();
        }

        [HttpPost]
        public async ValueTask<ActionResult<QueryResponse<UserResponseModel>>> QueryUsers(QueryRequest queryRequest, CancellationToken cancellationToken = default)
        {
            var result = await accountService.QueryUsersNoTrackingAsync(queryRequest, cancellationToken);

            return Ok(mapper.Map<QueryResponse<UserResponseModel>>(result));
        }

        [HttpPut]
        public async ValueTask<IActionResult> LockoutUser(string email, DateTime? until, CancellationToken cancellationToken = default)
        {
            var result = await accountService.SetUserLockoutAsync(email, until, cancellationToken);

            if (result.IsFailed)
                return BadRequest(result.ToErrorResponse());

            return Ok();
        }

    }
}
