using MastersOfCinema.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MastersOfCinema.ViewModels.Lists
{
    public class CListAvatarViewModel
    {
        /*public CList()
        {
            Movies = new List<ListMovies>();
        }
        public User User { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public List<ListMovies> Movies { get; set; }*/
        public CList Lists { get; set; }

        public List<Movie> Movies { get; set; }

    }

}
