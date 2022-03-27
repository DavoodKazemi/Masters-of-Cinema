using MastersOfCinema.Data.Entities;
using MastersOfCinema.ViewModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MastersOfCinema.Data
{
    public class Context : IdentityDbContext<User>
    {
        public Context(DbContextOptions<Context> options)
            : base(options)
        {
        }

        public DbSet<Director> Directors { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<MovieRating> MovieRatings { get; set; }
        public DbSet<Watchlist> Watchlists { get; set; }
        public DbSet<MovieLog> MovieLogs { get; set; }
        public DbSet<CList> Lists { get; set; }
        public DbSet<ListMovies> ListMovies { get; set; }
        public DbSet<Review> Review { get; set; }
        public DbSet<LikeReview> LikeReview { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
