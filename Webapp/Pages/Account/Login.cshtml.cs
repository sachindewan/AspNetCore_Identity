using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Webapp.Data.Account;

namespace Webapp.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;

        [BindProperty]
        public CredentialViewModel Credential { get; set; }

        [BindProperty]
        public IEnumerable<AuthenticationScheme> ExternalLoginProviders { get; set; }
        public LoginModel(SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
        }
        public async Task OnGet()
        {
            ExternalLoginProviders= await _signInManager.GetExternalAuthenticationSchemesAsync();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            var result = await _signInManager.PasswordSignInAsync(Credential.Email, Credential.Password, Credential.RememberMe, false);
            if (result.Succeeded) { return RedirectToPage("/Index"); }
            else
            {
                if (result.RequiresTwoFactor)
                {
                    //return RedirectToPage("/Account/LoginTwoFactor", new { Email = Credential.Email, RememberMe = Credential.RememberMe });
                    return RedirectToPage("/Account/LoginTwoFactorWithAuthenticator", new { RememberMe = Credential.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("Login", "User is locked out");
                }
                else
                {
                    ModelState.AddModelError("Login", "Failed to login");
                }
            }

            return Page();
        }
        public IActionResult OnPostLoginExternally(string provider)
        {
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, null);
            properties.RedirectUri = Url.Action("ExternalLoginCallback", "Account");

            return Challenge(properties, provider);
        }
    }
    public class CredentialViewModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remeber Me")]
        public bool RememberMe { get; set; }
    }
}
