﻿using MastersOfCinema.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace MastersOfCinema.Data.Entities
{
    public class Watchlist
    {
        [Key]
        public int Id { get; set; }
        public User User { get; set; }
        public int MovieId { get; set; }
    }
}
