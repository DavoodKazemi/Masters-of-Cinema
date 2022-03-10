using MastersOfCinema.Data.Entities;
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
        public IEnumerable<CList> Lists { get; set; }
        //public IEnumerable<MovieListViewModel> movieList { get; set; }
        public bool IsFirstPage { get; set; }
    }
}


//View Model for display the list of custom lists