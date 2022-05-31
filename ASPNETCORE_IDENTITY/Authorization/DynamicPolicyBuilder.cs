using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace ASPNETCORE_IDENTITY.Authorization
{
    public class SecurityLevelAttribute:AuthorizeAttribute{
        public int Level { get; set; }

        public SecurityLevelAttribute(int level)
        {
            Policy = $"{DynamicPolicies.SecurityLevel}.{level}";
        }
    }

    public class ClaimSecurityAttribute : AuthorizeAttribute
    {
        public ClaimSecurityAttribute(string value)
        {
            Policy = $"{DynamicPolicies.Claim}.{value}";
        }
    }
    public class SecurityLevelRequirement : IAuthorizationRequirement
    {
        public int Level { get; } 
        public SecurityLevelRequirement(int level)
        {
            Level = level;
        }
    }

    public class SecurityLevelHandler : AuthorizationHandler<SecurityLevelRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SecurityLevelRequirement requirement)
        {
            var claimValue =
                Convert.ToInt32(context.User.Claims.FirstOrDefault(x => x.Type == DynamicPolicies.SecurityLevel)?.Value ?? "0");
            if (requirement.Level <= claimValue)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
    public static class DynamicPolicies
    {
        public static IEnumerable<string> Get()
        {
            yield return Claim;
            yield return Rank;
            yield return SecurityLevel;
        }
        public const string Claim = "Claim";
        public const string Rank = "Rank";
        public const string SecurityLevel = "SecurityLevel";
    }

    public static class DynamicAuthorizationPolicyFactory
    {
        public static AuthorizationPolicy Create(string policyName)
        {
            var parts = policyName.Split('.');
            var type = parts.First();
            var value = parts.Last();
            switch (type)
            {
                case DynamicPolicies.Claim:
                    return new AuthorizationPolicyBuilder().RequireClaim(type,value).Build();
                case DynamicPolicies.Rank:
                    return new AuthorizationPolicyBuilder().RequireClaim(type, value).Build();
                case DynamicPolicies.SecurityLevel:
                    return new AuthorizationPolicyBuilder()
                        .AddRequirements(new SecurityLevelRequirement(Convert.ToInt32(value))).Build();
                default:
                    return null;
            }
        }
    }

    public class CustomAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        public CustomAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
        {
        }

        //{type}.{value}
        public override Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            foreach (var customPolicy in DynamicPolicies.Get())
            {
                if (policyName.StartsWith(customPolicy))
                {
                    var policy = DynamicAuthorizationPolicyFactory.Create(policyName);
                    return Task.FromResult(policy);
                }
            }
            return base.GetPolicyAsync(policyName);
        }
    }
}
