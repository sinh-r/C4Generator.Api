using Microsoft.AspNetCore.Authorization;

namespace C4Generator.Api.Authorization;

public sealed class AdminPolicyHandler : AuthorizationHandler<AdminRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminRequirement requirement)
    {
        var isAdmin = context.User.HasClaim(c =>
            c.Type == "role" && c.Value.Equals("admin", StringComparison.OrdinalIgnoreCase));

        if (isAdmin)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
