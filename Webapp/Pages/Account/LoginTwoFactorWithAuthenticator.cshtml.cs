using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Webapp.Data.Account;

namespace Webapp.Pages.Account
{
    public class LoginTwoFactorWithAuthenticatorModel : PageModel
    {
        [BindProperty]
        public AuthenticatorMFA AuthenticatorMfa { get; set; }

        private readonly SignInManager<User> _signInManager;

        public LoginTwoFactorWithAuthenticatorModel(SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
            AuthenticatorMfa = new AuthenticatorMFA();
        }
        public void OnGet(bool rememberMe)
        {
            AuthenticatorMfa.RememberMe = rememberMe;
            AuthenticatorMfa.SecurityCode = string.Empty;
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(AuthenticatorMfa.SecurityCode,
                AuthenticatorMfa.RememberMe, false);
            if (result.Succeeded) { return RedirectToPage("/Index"); }
            else
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("Authenticator2FA", "User is locked out");
                }
                else
                {
                    ModelState.AddModelError("Authenticator2FA", "Failed to login");
                }
            }

            return Page();
        }
    }

    public class AuthenticatorMFA
    {
        [Required]
        [Display(Name = "Code")]
        public string SecurityCode { get; set; }

        public bool RememberMe { get; set; }
    }
}
