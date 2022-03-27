using MastersOfCinema.Data;
using MastersOfCinema.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MastersOfCinema.Views.ViewComponents
{
    public class UserAvatarViewComponent
        : ViewComponent
    {
        private readonly ICinemaRepository _repository;
        private readonly Context _context;
        private readonly string _userId;

        public UserAvatarViewComponent(ICinemaRepository repository, /*IHttpContextAccessor httpContextAccessor,*/
            Context context)
        {
            _repository = repository;
            _context = context;
            //_userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        }

        public IViewComponentResult Invoke(string userId)
        {
            //FIX LATER - IF LOGGED IN >> THE USER SHOULD BE DISTINGUISHEd
            //(EX. for displaying his review on top, or for editing)


            //User user = _context.Users.Where(i => i.Id == _userId).FirstOrDefault();
            //Fix later - shouldn't be executed if db is not seeded
            //if (_context.Users.Any(x => x.Id == user.Id))
            //{
            //var Username = user.FirstName ?? user.UserName;
            User user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
            //user.ProfilePicture = Convert.ToBase64String(UserManager.GetUserAsync(User).Result.ProfilePicture);
                return View("Default", user);
            //}
            //Fix later - shouldn't be executed if no user is logged in
            //return View("Default");
        }
    }
}
