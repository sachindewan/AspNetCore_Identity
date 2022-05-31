using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using ASPNETCORE_IDENTITY.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ASPNETCORE_IDENTITY.Pages.Account
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public Credential Credential { get; set; }

        public void OnGet()
        {            
        }

        public async Task<IActionResult>  OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            if (Credential.UserName == "admin" && Credential.Password == "password")
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, Credential.UserName),
                    new Claim(ClaimTypes.Email, "admin@mywebsite.com"),
                    new Claim("Department","Hr"),
                    new Claim("Admin", "true"),
                    new Claim("Manager", "true"),
                    new Claim(DynamicPolicies.Claim, "true"),
                    new Claim("EmploymentDate", "20/12/2021")
                };
                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                var principle = new ClaimsPrincipal(identity);

                var authProperties = new AuthenticationProperties()
                {
                    IsPersistent = Credential.RememberMe
                };
                await HttpContext.SignInAsync("MyCookieAuth", principle, authProperties);
                return RedirectToPage("/index");
            }
            return Page();
        }
    }
    
}
