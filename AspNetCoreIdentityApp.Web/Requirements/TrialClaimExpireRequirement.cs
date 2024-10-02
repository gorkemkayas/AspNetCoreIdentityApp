using Microsoft.AspNetCore.Authorization;

namespace AspNetCoreIdentityApp.Web.Requirements
{
    public class TrialClaimExpireRequirement : IAuthorizationRequirement
    {
    }
    public class TrialClaimExpireRequirementHandler : AuthorizationHandler<TrialClaimExpireRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TrialClaimExpireRequirement requirement)
        {
            if(!context.User.HasClaim(x => x.Type == "TrialClaim"))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            var trialExpireDate = context.User.FindFirst("TrialClaim");

            if (DateTime.Now > Convert.ToDateTime(trialExpireDate))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
