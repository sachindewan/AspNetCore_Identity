using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;

namespace ASPNETCORE_IDENTITY.Authorization
{
    public class HrManagerProbationRequirement : IAuthorizationRequirement
    {
        public int ProbationMonth { get; }

        public HrManagerProbationRequirement(int probationMonth)
        {
            ProbationMonth = probationMonth;
        }
    }

    public class HrManagerProbationRequirementHandler : AuthorizationHandler<HrManagerProbationRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HrManagerProbationRequirement requirement)
        {
            if (!context.User.HasClaim(x => x.Type == "EmploymentDate"))
            {
                return Task.CompletedTask;
            }

            var var = context.User.FindFirst(x => x.Type == "EmploymentDate")?.Value;
            if (var != null)
            {
                var empDate = DateTime.Parse(var);
                var period = DateTime.Now - empDate;
                if (period.Days > 30 * requirement.ProbationMonth)
                    context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
