using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace simple_aspnet_auth
{
  public class AdminRequirement : IAuthorizationRequirement
  {
    public AdminRequirement(bool includeSuperUser = false)
    {
      IncludeSuperUser = includeSuperUser;
    }

    public bool IncludeSuperUser { get; set; }

  }

  public class AdminHandler : AuthorizationHandler<AdminRequirement>
  {
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminRequirement requirement)
    {
      var user = context.User;
      var identity = user.Identity;
      var isAuthenticated = identity.IsAuthenticated;

      if (!isAuthenticated)
      {
        return Task.CompletedTask;
      }

      var claims = user.Claims;
      var claimNames = from x in claims select x.Type;
      var groups = new List<string> { GroupNames.Admins };

      if (requirement.IncludeSuperUser)
      {
        groups.Add(GroupNames.SuperUsers);
      }

      var isAdmin = claimNames.Any(groups.Contains);

      if (!isAdmin)
      {
        return Task.CompletedTask;
      }

      context.Succeed(requirement);

      return Task.CompletedTask;

    }

  }

}
