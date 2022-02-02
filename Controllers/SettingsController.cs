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

        //remove all StatusMessage sh*t

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
            TempData["ConfirmMessage2"] = "Your password has been changed.";
            return View(settingsViewModel);
        }

        //End ChangePassword

        //Email - Only change the address (doesn't send confirmation email)
        [HttpPost]
        public async Task<IActionResult> Email(ChangeEmailViewModel changeEmailViewModel)
        {
            var user = await _userManager.GetUserAsync(User);
            //Getting new Email of the user, Entered in the form
            var newInfo = changeEmailViewModel.NewEmail;

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                return View(changeEmailViewModel);
            }

            var email = await _userManager.GetEmailAsync(user);
            if (newInfo != email)
            {
                user.Email = newInfo;
                await _userManager.UpdateAsync(user);
                TempData["ConfirmMessage2"] = "Your Email address is updated.";
                return View(changeEmailViewModel);
            }

            TempData["ConfirmMessage2"] = "Your Email has not been changed.";
            return View(changeEmailViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Email()
        {
            var user2 = await _userManager.GetUserAsync(User);
            if (user2 == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var Input2Email = new ChangeEmailViewModel
            {
                CurrentUser = user2,
                NewEmail = user2.Email

            };
            Input2Email.CurrentUser.Email = user2.Email;

            //await LoadAsync(user);
            return View(Input2Email);
        }

        //End Email

        public string Username { get; set; }



            [HttpGet]
            public async Task<IActionResult> Index()
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

            var userInfo = new User
            {
                PhoneNumber = user.PhoneNumber,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ProfilePicture = user.ProfilePicture,
                UsernameChangeLimit = user.UsernameChangeLimit
                };

                ProfileViewModel profileViewModel = new ProfileViewModel();
                profileViewModel.CurrentUser = userInfo;

                if(user.UsernameChangeLimit > 0)
                {
                    TempData["UserNameChangeLimitMessage2"] = 
                                    $"You can change your username {user.UsernameChangeLimit} more time(s)!";
                }
                return View(profileViewModel);
            }

        [HttpPost]
        public async Task<IActionResult> Index(ProfileViewModel profileViewModel)
        {
            //Getting original propeties of the user
            var user = await _userManager.GetUserAsync(User);
            //Getting new propeties of the user, Entered in the form
            var newInfo = profileViewModel.CurrentUser;

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            string ConfirmMessage = "";

            var errors = ModelState
           .Where(x => x.Value.Errors.Count > 0)
           .Select(x => new { x.Key, x.Value.Errors })
           .ToArray();

            if (!ModelState.IsValid)
            {
                return View(profileViewModel);
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            if (newInfo.FirstName == user.FirstName && newInfo.LastName == user.LastName && Request.Form.Files.Count == 0
                && newInfo.PhoneNumber == phoneNumber && newInfo.UserName == user.UserName)
            {
                TempData["ConfirmMessage2"] = "You have made no change to save!";
                profileViewModel.CurrentUser.ProfilePicture = user.ProfilePicture;
            }
            else
            {
                //First name
                if (user.FirstName != newInfo.FirstName)
                {
                    user.FirstName = newInfo.FirstName;
                    ConfirmMessage = "First name";
                }
                //Last name
                if (user.LastName != newInfo.LastName)
                {
                    user.LastName = newInfo.LastName;
                    ConfirmMessage += 
                        string.IsNullOrEmpty(ConfirmMessage) ? "Last name" : ", Last name";
                }
                //Phone number
                if (newInfo.PhoneNumber != phoneNumber)
                {
                    var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, newInfo.PhoneNumber);
                    if (!setPhoneResult.Succeeded)
                    {
                        ConfirmMessage = "Error: Unexpected error when trying to set phone number.";
                        return View(profileViewModel);
                    }
                    ConfirmMessage +=
                        string.IsNullOrEmpty(ConfirmMessage) ? "Phone number" : ", Phone number";
                }
                //Picture
                if (Request.Form.Files.Count > 0)
                {
                    IFormFile file = Request.Form.Files.FirstOrDefault();
                    using (var dataStream = new MemoryStream())
                    {
                        await file.CopyToAsync(dataStream);
                        user.ProfilePicture = dataStream.ToArray();
                    }
                    ConfirmMessage +=
                        string.IsNullOrEmpty(ConfirmMessage) ? "Profile picture" : ", Profile picture";
                }

                //User name
                if (user.UsernameChangeLimit > 0)
                {
                    if (newInfo.UserName != user.UserName)
                    {
                        var userNameExists = await _userManager.FindByNameAsync(newInfo.UserName);
                        if (userNameExists != null)
                        {
                            TempData["UserNameChangeLimitMessage2"] = "Error: User name already taken. Select a different user name.";
                            return View(profileViewModel);
                        }
                        var setUserName = await _userManager.SetUserNameAsync(user, newInfo.UserName);
                        if (!setUserName.Succeeded)
                        {
                            TempData["UserNameChangeLimitMessage2"] = "Error: Unexpected error when trying to set user name.";
                            return View(profileViewModel);
                        }
                        else
                        {
                            ConfirmMessage +=
                            string.IsNullOrEmpty(ConfirmMessage) ? "User name" : ", User name";
                            user.UsernameChangeLimit -= 1;
                        }
                    }
                }
                await _userManager.UpdateAsync(user);
                TempData["ConfirmMessage2"] = "Your profile has been updated in the following properties: "
                    + ConfirmMessage + ".";
            }

            //Display the profile picture (either it's changed or not)
            profileViewModel.CurrentUser.ProfilePicture = user.ProfilePicture;

            await _signInManager.RefreshSignInAsync(user);
            return View(profileViewModel);
        }
    }
}//468
