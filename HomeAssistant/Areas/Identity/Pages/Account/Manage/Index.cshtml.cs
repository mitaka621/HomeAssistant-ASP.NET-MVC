// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using HomeAssistant.Core.Contracts;
using HomeAssistant.Core.Services;
using HomeAssistant.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HomeAssistant.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<HomeAssistantUser> _userManager;
        private readonly SignInManager<HomeAssistantUser> _signInManager;
        private readonly IPFPService _pfpService;

        public IndexModel(
            UserManager<HomeAssistantUser> userManager,
            SignInManager<HomeAssistantUser> signInManager,
            IPFPService ipFPService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _pfpService = ipFPService;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            public IFormFile ImageFile { get; set; }
        }

        private async Task LoadAsync(HomeAssistantUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber
            };

            var imageData = await _pfpService.GetImage(user.Id);

            if (imageData != null && imageData.Length > 0)
            {

                ViewData["ImageData"] = imageData;
            }
            else
            {
                ViewData["ImageData"] = null;
            }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);

           
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Task uploadPfpTask = null;
            if (Input.ImageFile != null && Input.ImageFile.Length > 0)
            {
                using (var stream = new MemoryStream())
                {
                    Input.ImageFile.CopyTo(stream);
                    var imageData = stream.ToArray();

                    var userId = _userManager.GetUserId(User);

                    uploadPfpTask = _pfpService.SaveImage(userId, imageData);
                }

            }

            
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult =await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);

                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            if (uploadPfpTask!=null)
            {
                await uploadPfpTask;
            }

            await _signInManager.RefreshSignInAsync(user);
            
            StatusMessage = "Your profile has been updated";
       
            return LocalRedirect("/Identity/Account/Manage");
        }
    }
}
