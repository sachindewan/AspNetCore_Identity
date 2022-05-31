using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Webapp.Data.Account;

namespace Webapp.Pages.Account
{
    [Authorize]
    public class UserProfileModel : PageModel
    {
        [BindProperty]
        public UserProfile UserProfile { get; set; }
        [BindProperty]
        public string SuccessMessage { get; set; }
        private readonly UserManager<User> userManager;
        public UserProfileModel(UserManager<User> userManager)
        {
            this.userManager = userManager;
            UserProfile = new UserProfile();
            SuccessMessage = string.Empty;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            var (user, departmentClaims, positionClaims) = await GetUserInfoAsync();
            UserProfile.Email = User.Identity.Name;
            UserProfile.Position = positionClaims?.Value;
            UserProfile.Department = departmentClaims?.Value;
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            var (user, departmentClaims, positionClaims) = await GetUserInfoAsync();
            try
            {
                await userManager.ReplaceClaimAsync(user, departmentClaims,
                    new Claim(departmentClaims.Type, UserProfile.Department));
                await userManager.ReplaceClaimAsync(user, positionClaims,
                    new Claim(positionClaims.Type, UserProfile.Position));
            }
            catch (Exception e)
            {
                ModelState.AddModelError("UserProfile","Error occured while saving user profile");
            }

            this.SuccessMessage = "The user profile saved successfully.";
            return Page();
        }
        private async Task<(User,Claim,Claim)> GetUserInfoAsync()
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var claims = await userManager.GetClaimsAsync(user);
            var departmentClaims = claims.FirstOrDefault(x => x.Type == "Department");
            var positionClaims = claims.FirstOrDefault(x => x.Type == "Position");
            return (user, departmentClaims, positionClaims);
        }
    }

    public class UserProfile
    {
        public string Email { get; set; }
        [Required]
        public string Department { get; set; }
        [Required]
        public string Position { get; set; }
    }
}
