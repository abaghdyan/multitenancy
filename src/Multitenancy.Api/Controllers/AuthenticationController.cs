using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Multitenancy.Api.Controllers.Base;
using Multitenancy.Common.Multitenancy;
using Multitenancy.Services.Abstractions;
using Multitenancy.Services.Models;

namespace Multitenancy.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthenticationController : TenantBaseController
{
    private readonly IAuthenticationService _authService;

    public AuthenticationController(IAuthenticationService authService,
        UserContext userContext)
        : base(userContext)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost("signIn")]
    public async Task<ActionResult<string>> SignInAsync(UserSignInModel userSignInModel)
    {
        if (userSignInModel == null)
        {
            return BadRequest();
        }

        var user = await _authService.SignInUserAsync(userSignInModel);

        if (user == null)
        {
            return Unauthorized();
        }

        var token = _authService.GenerateAccessToken(user);
        return Ok(token);
    }
}