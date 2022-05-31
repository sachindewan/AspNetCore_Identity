using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Webapp.Data.Account;
using Webapp.Services;

namespace Webapp.Pages.Account
{
    public class LoginTwoFactorModel : PageModel
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly IEmailService _emailService;
        [BindProperty]
        public TwoFactorLogin TwoFactorLogin { get; set; }
        public LoginTwoFactorModel(UserManager<User> userManager,IEmailService emailService,SignInManager<User>signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            _emailService = emailService;
            TwoFactorLogin = new();
        }
        public async Task OnGetAsync(string email,bool rememberMe)
        {
            var user = await userManager.FindByEmailAsync(email);
            var token = await userManager.GenerateTwoFactorTokenAsync(user, "Email");
            TwoFactorLogin.RememberMe = rememberMe;
            await _emailService.SendAsync("sachindewan12@gmail.com", "sachindewan12@gmail.com",
                "Two factor authentication",
                $"please verify the two factor authentication for successful login {token}");
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var result = await signInManager.TwoFactorSignInAsync("Email",TwoFactorLogin.MFAToken,TwoFactorLogin.RememberMe,false);
            if (result.Succeeded) { return RedirectToPage("/Index"); }
            else
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("Login2FA", "User is locked out");
                }
                else
                {
                    ModelState.AddModelError("Login2FA", "Failed to login");
                }
            }

            return Page();
        }
    }

    public class TwoFactorLogin
    {
        public string MFAToken { get; set; }
        public bool RememberMe { get; set; }
    }
}
