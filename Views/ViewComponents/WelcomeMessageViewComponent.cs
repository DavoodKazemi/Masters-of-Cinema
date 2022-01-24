using MastersOfCinema.Data;
using MastersOfCinema.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MastersOfCinema.Views.ViewComponents
{
    public class WelcomeMessageViewComponent
        :ViewComponent
    {
        private readonly ICinemaRepository _repository;
        private readonly Context _context;
        private readonly string _userId;

        public WelcomeMessageViewComponent(ICinemaRepository repository, IHttpContextAccessor httpContextAccessor,
            Context context)
        {
            _repository = repository;
            _context = context;
            _userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        }

        public IViewComponentResult Invoke()
        {
            User user = _context.Users.Where(i => i.Id == _userId).FirstOrDefault();
            var Username = user.FirstName;
            return View("Default", Username);
        }
    }
}
