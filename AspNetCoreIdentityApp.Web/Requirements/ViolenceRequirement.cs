using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AspNetCoreIdentityApp.Web.Requirements
{
    public class ViolenceRequirement : IAuthorizationRequirement
    {
        public int ThresholdAge { get; set; }
    }

    public class ViolenceRequirementHandler : AuthorizationHandler<ViolenceRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ViolenceRequirement requirement)
        {
            if(!context.User.HasClaim(x=> x.Type == "Birthdate"))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            Claim userBirthDate = context.User.FindFirst("Birthdate")!;

            var now = DateTime.Now;
            var birthDate = Convert.ToDateTime(userBirthDate.Value);
            var age = now.Year - birthDate.Year;

            if (birthDate > now.AddYears(-age)) age--;

            if(requirement.ThresholdAge > age)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }

}
