using MastersOfCinema.Data.Entities;
using MastersOfCinema.ViewModels.Lists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MastersOfCinema.ViewModels
{
    public class CListsViewModel
    {
        public User User { get; set; }
        public int listCount { get; set; }

        //public IEnumerable<MovieListViewModel> movieList { get; set; }
        public bool IsFirstPage { get; set; }
        public IEnumerable<CListAvatarViewModel> Lists { get; set; }
    }
}


//View Model for display the list of custom lists