using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRCoder;
using Webapp.Data.Account;

namespace Webapp.Pages.Account
{
    [Authorize]
    public class AuthenticatorWithMFASetupModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        [BindProperty]
        public SetupMFAViewModel ViewModel { get; set; }
        [BindProperty]
        public bool Succeded { get; set; }
        public AuthenticatorWithMFASetupModel(UserManager<User> userManager)
        {
            _userManager = userManager;
            ViewModel = new SetupMFAViewModel();
        }
        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(base.User);
            await _userManager.ResetAuthenticatorKeyAsync(user);
            var key = await _userManager.GetAuthenticatorKeyAsync(user);
            ViewModel.Key = key;
            ViewModel.QRCodeBytes = GenerateQRCodeBytes(_userManager.Options.Tokens.AuthenticatorTokenProvider, key, user.Email);
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            var user = await _userManager.GetUserAsync(base.User);
            var verified = await _userManager.VerifyTwoFactorTokenAsync(user,
                _userManager.Options.Tokens.AuthenticatorTokenProvider, ViewModel.SecuretyCode);
            if (verified)
            {
                Succeded = true;
            }
            else
            {
                ModelState.AddModelError("AuthenticatorSetup","Some thing went wrong with authenticator setup.");
            }
            return Page();
        }
        private Byte[] GenerateQRCodeBytes(string provider, string key, string userEmail)
        {
            var qrCodeGenerater = new QRCodeGenerator();
            var qrCodeData = qrCodeGenerater.CreateQrCode(
                $"otpauth://totp/{provider}:{userEmail}?secret={key}&issuer={provider}",
                QRCodeGenerator.ECCLevel.Q);
            var qrCode = new QRCode(qrCodeData);
            var qrCodeImage = qrCode.GetGraphic(20);

            return BitmapToByteArray(qrCodeImage);
        }

        private Byte[] BitmapToByteArray(Bitmap image)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }
    }

    public class SetupMFAViewModel
    {
        public string Key { get; set; }
        [Required]
        [Display(Name = "Code")]
        public string SecuretyCode { get; set; }
        public Byte[] QRCodeBytes { get; set; }
    }
}
