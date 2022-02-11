using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MastersOfCinema.ViewModels
{
    public class MovieListViewModel
    {
        public int listCount { get; set; }
        public IEnumerable<Movie> Movies { get; set; }
        public User CurrentUser { get; set; }
        public bool IsFirstPage { get; set; }
    }
}
