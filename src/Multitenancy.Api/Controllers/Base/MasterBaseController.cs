using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Multitenancy.Common.Constants;

namespace Multitenancy.Api.Controllers.Base;

[Authorize(AuthenticationSchemes = ApplicationAuthSchemes.AdminFlow)]
public class MasterBaseController : ControllerBase
{
}
