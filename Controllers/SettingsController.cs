using MastersOfCinema.Data;
using MastersOfCinema.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace MastersOfCinema.Controllers
{
    public class SettingsController : Controller
    {


        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly Context _context;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<SettingsController> _logger;

        public SettingsController(
                UserManager<User> userManager,
                SignInManager<User> signInManager,
                Context context,
                IEmailSender emailSender,
                ILogger<SettingsController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _emailSender = emailSender;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> DownloadPersonalData()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            _logger.LogInformation("User with ID '{UserId}' asked for their personal data.", _userManager.GetUserId(User));

            // Only include personal data for download
            var personalData = new Dictionary<string, string>();
            var personalDataProps = typeof(User).GetProperties().Where(
                            prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
            foreach (var p in personalDataProps)
            {
                personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");
            }

            var logins = await _userManager.GetLoginsAsync(user);
            foreach (var l in logins)
            {
                personalData.Add($"{l.LoginProvider} external login provider key", l.ProviderKey);
            }

            Response.Headers.Add("Content-Disposition", "attachment; filename=PersonalData.json");
            return new FileContentResult(JsonSerializer.SerializeToUtf8Bytes(personalData), "application/json");
        }

        public async Task<IActionResult> PersonalData()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            return View();
        }

        //End User Data
        //Change Password
        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                return RedirectToPage("./SetPassword");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(SettingsViewModel settingsViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, settingsViewModel.OldPassword, settingsViewModel.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(settingsViewModel);
            }

            await _signInManager.RefreshSignInAsync(user);
            _logger.LogInformation("User changed their password successfully.");
            StatusMessage = "Your password has been changed.";

            return View(settingsViewModel);
        }

        //End ChangePassword

        //Email - not working now (neither the razor does)

        /*private async Task LoadAsync(User user)
        {
            var email = await _userManager.GetEmailAsync(user);
            Email = email;

            Input = new InputModel
            {
                NewEmail = email,
            };

            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
        }*/

        [HttpGet]
        public async Task<IActionResult> Email()
        {
            var user2 = await _userManager.GetUserAsync(User);
            if (user2 == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var Input2Email = new SettingsViewModel
            {
                CurrentUser = user2,
                NewEmail = user2.Email

            };
            Input2Email.CurrentUser.Email = user2.Email;

            //await LoadAsync(user);
            return View(Input2Email);
        }

        [HttpPost]
        public async Task<IActionResult> Email(SettingsViewModel settingsViewModel)
        {
            var user = await _userManager.GetUserAsync(User);
            
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                //await LoadAsync(user);
                return View();
            }

            var email = await _userManager.GetEmailAsync(user);
            if (settingsViewModel.NewEmail != email)
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateChangeEmailTokenAsync(user, settingsViewModel.NewEmail);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmailChange",
                    pageHandler: null,
                    values: new { userId = userId, email = settingsViewModel.NewEmail, code = code },
                    protocol: Request.Scheme);
                await _emailSender.SendEmailAsync(
                    settingsViewModel.NewEmail,
                    "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                StatusMessage = "Confirmation link to change email sent. Please check your email.";

                settingsViewModel.CurrentUser = user;


                return View(settingsViewModel);
            }

            StatusMessage = "Your email is unchanged.";
            return View(settingsViewModel);
        }

        //End Email


        public string Username { get; set; }

            [TempData]
            public string StatusMessage { get; set; }

            [BindProperty]
            public InputModel Input { get; set; }

            public class InputModel
            {
                [Display(Name = "First Name")]
                public string FirstName { get; set; }
                [Display(Name = "Last Name")]
                public string LastName { get; set; }
                [Display(Name = "Username")]
                public string Username { get; set; }
                [Phone]
                [Display(Name = "Phone number")]
                public string PhoneNumber { get; set; }
                [Display(Name = "Profile Picture")]
                public byte[] ProfilePicture { get; set; }
            }

            private async Task LoadAsync(User user)
            {
                var userName = await _userManager.GetUserNameAsync(user);
                var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
                var firstName = user.FirstName;
                var lastName = user.LastName;
                var profilePicture = user.ProfilePicture;
                Username = userName;
                Input = new InputModel
                {
                    PhoneNumber = phoneNumber,
                    Username = userName,
                    FirstName = firstName,
                    LastName = lastName,
                    ProfilePicture = profilePicture
                };
            }

            [HttpGet]
            public async Task<IActionResult> Index()
            {
                var userName = await _userManager.GetUserAsync(User);

                var Input2 = new User
                {
                    PhoneNumber = userName.PhoneNumber,
                    UserName = userName.UserName,
                    FirstName = userName.FirstName,
                    LastName = userName.LastName,
                    ProfilePicture = userName.ProfilePicture
                };

                var user = await _userManager.GetUserAsync(User);
                    if (user == null)
                    {
                        return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                    }

                //await LoadAsync(user);

                return View(Input2);
            }

            /*public async Task<IActionResult> OnGetAsync()
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                await LoadAsync(user);
                return Page();
            }*/

            [HttpPost]
            public async Task<IActionResult> Index(User user2)
            {

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                var firstName = user.FirstName;
                var lastName = user.LastName;
                if (Input.FirstName != firstName)
                {
                    user.FirstName = Input.FirstName;
                    await _userManager.UpdateAsync(user);
                }
                if (Input.LastName != lastName)
                {
                    user.LastName = Input.LastName;
                    await _userManager.UpdateAsync(user);
                }

                if (Request.Form.Files.Count > 0)
                {
                    IFormFile file = Request.Form.Files.FirstOrDefault();
                    using (var dataStream = new MemoryStream())
                    {
                        await file.CopyToAsync(dataStream);
                        user.ProfilePicture = dataStream.ToArray();
                    }
                    await _userManager.UpdateAsync(user);
                }


                if (!ModelState.IsValid)
                {
                    await LoadAsync(user);
                    return View();
                }

                var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
                if (Input.PhoneNumber != phoneNumber)
                {
                    var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                    if (!setPhoneResult.Succeeded)
                    {
                        StatusMessage = "Unexpected error when trying to set phone number.";
                        return View();
                    }
                }

                await _signInManager.RefreshSignInAsync(user);
                StatusMessage = "Your profile has been updated";
                return View(user2);
            }
    }
}
