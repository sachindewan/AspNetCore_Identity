﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Webapp.Data.Account;

namespace Webapp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<User> _signInManager;
        public AccountController(SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
        }
        public async Task<IActionResult> ExternalLoginCallback()
        {
            var loginInfo = await _signInManager.GetExternalLoginInfoAsync();
            var emailClaim = loginInfo.Principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
            var userClaim = loginInfo.Principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name);

            if (emailClaim != null && userClaim != null)
            {
                var user = new User { Email = emailClaim.Value, UserName = userClaim.Value };
                await _signInManager.SignInAsync(user, false);
            }

            return RedirectToPage("/Index");
        }
    }
}
