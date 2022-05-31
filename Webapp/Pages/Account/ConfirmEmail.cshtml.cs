using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Webapp.Data.Account;

namespace Webapp.Pages.Account
{
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        public ConfirmEmailModel(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        [BindProperty]
        public string Message { get; set; }
        public async Task<IActionResult> OnGetAsync(string userId,string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var emailConfirm = await _userManager.ConfirmEmailAsync(user, token);
                if (emailConfirm.Succeeded)
                {
                    Message = "Email address is successully confirmed, now you can try to login.";
                    return Page();
                }
            }
              Message = "Failed to validate email.";
              return Page();
            
        }
    }
}
