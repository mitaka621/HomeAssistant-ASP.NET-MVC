﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using HomeAssistant.Infrastructure.Data;
using HomeAssistant.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HomeAssistant.Areas.Identity.Pages.Account
{
    [SkipStatusCodePages]
    public class LoginModel : PageModel
    {
        private readonly SignInManager<HomeAssistantUser> _signInManager;
        private readonly HomeAssistantDbContext _homeAssistantDbContext;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(SignInManager<HomeAssistantUser> signInManager, ILogger<LoginModel> logger, HomeAssistantDbContext homeAssistantDbContext)
        {
            _signInManager = signInManager;
            _logger = logger;
            _homeAssistantDbContext = homeAssistantDbContext;
        }

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
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

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
            [Required]
            [DisplayName("Username")]
            public string UserName { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }


        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            var record = await _homeAssistantDbContext.BlacklistedIPs.FirstOrDefaultAsync(x => x.Ip == HttpContext.Connection.RemoteIpAddress.ToString());

            if (record != null && record.Count >= 10)
            {
                return StatusCode(403);
            }

            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;



            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();



            if (ModelState.IsValid)
            {
                var user = await _signInManager
                            .UserManager
                            .Users
                            .FirstOrDefaultAsync(x => x.UserName.ToUpper() == Input.UserName.ToUpper());

                if (user != null && user.IsDeleted)
                {
                    _logger.LogWarning("User account is deleted.");
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }


                var result = await _signInManager.PasswordSignInAsync(Input.UserName, Input.Password, Input.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var record = await _homeAssistantDbContext.BlacklistedIPs.FirstOrDefaultAsync(x => x.Ip == HttpContext.Connection.RemoteIpAddress.ToString());


                    if (record != null)
                    {
                        record.Count = 0;
                    }

                    _logger.LogInformation("User logged in.");
                    _homeAssistantDbContext.Users.First(x => x.Id == user.Id).ClientIpAddress = HttpContext.Connection.RemoteIpAddress.ToString();
                    await _homeAssistantDbContext.SaveChangesAsync();

                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");

                    var record = await _homeAssistantDbContext.BlacklistedIPs.FirstOrDefaultAsync(x => x.Ip == HttpContext.Connection.RemoteIpAddress.ToString());


                    if (record == null)
                    {
                        record = new BlacklistedIp()
                        {
                            Ip = HttpContext.Connection.RemoteIpAddress.ToString()
                        };

                        _homeAssistantDbContext.BlacklistedIPs.Add(record);
                    }

                    record.Count++;
                    record.LastTry = DateTime.Now;

                    await _homeAssistantDbContext.SaveChangesAsync();

                    if (record.Count >= 10)
                    {
                        return StatusCode(403);
                    }

                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
