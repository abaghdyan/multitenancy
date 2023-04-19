using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Multitenancy.Common.Constants;
using Multitenancy.Common.Multitenancy;

namespace Multitenancy.Api.Controllers.Base;

[Authorize(AuthenticationSchemes = ApplicationAuthSchemes.TenantBearer)]
public class TenantBaseController : ControllerBase
{
    public UserContext UserContext { get; }

    public TenantBaseController(UserContext userContext)
    {
        UserContext = userContext;
    }
}
