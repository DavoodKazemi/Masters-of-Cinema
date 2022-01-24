using MastersOfCinema.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MastersOfCinema.Views.ViewComponents
{
    public class WelcomeMessageViewComponent
        :ViewComponent
    {
        private readonly ICinemaRepository _repository;

        public WelcomeMessageViewComponent(ICinemaRepository repository)
        {
            _repository = repository;
        }

        public IViewComponentResult Invoke()
        {
            var Username = _repository.CurrnentUserName();
            return View("Default", Username);
        }
    }
}
