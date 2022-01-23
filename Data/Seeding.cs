using MastersOfCinema.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MastersOfCinema.Data
{
    public class Seeding
    {
        private readonly Context ctx;
        private readonly IWebHostEnvironment env;
        private readonly UserManager<User> _userManager;

        public Seeding(Context ctx, IWebHostEnvironment env, UserManager<User> userManager)
        {
            this.ctx = ctx;
            this.env = env;
            _userManager = userManager;
        }

        public async Task SeedAsync()
        {
            ctx.Database.EnsureCreated();

            User user = await _userManager.FindByEmailAsync("SacredCode1@gmail.com");
            if (user == null)
            {
                user = new User
                {
                    FirstName = "Davood",
                    LastName = "Kazemi",
                    Email = "SacredCode1@gmail.com",
                    UserName = "SacredCode1@gmail.com",
                };


                var result = await _userManager.CreateAsync(user, "P@ssw0rd!");
                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create new user in seeder");
                }
            }
            if (!ctx.Directors.Any())
            {
                //Then we need to create the sample data
                var filePath = Path.Combine(env.ContentRootPath, "Data/Default/director.json");
                var json = File.ReadAllText(filePath);
                var directors = JsonSerializer.Deserialize<IEnumerable<Director>>(json);

                ctx.Directors.AddRange(directors);


            /*var director = new DirectorViewModel
             * ()
            {
                Id = 1,
                Name = "Yasujirō Ozu",
                Country = "Japan",
                Bio = "Yasujirō Ozu was a Japanese film director and screenwriter.",
                ImageName = "Yasujirō Ozu_e1d4.jpg"
            };

            var movie = new MovieViewModel()
            {
                Title = "Late spring",
                DirectorViewModelId = 1,
                Year = "1949",
                Description = "desc",
                ImageName = "Late Spring_eadf.jpg"
            };

            ctx.Movie.Add(movie);
            ctx.Director.Add(director);*/

            ctx.SaveChanges();
            }


            if (!ctx.Movies.Any())
            {
                //Then we need to create the sample data
                var filePath = Path.Combine(env.ContentRootPath, "Data/Default/movie.json");
                var json = File.ReadAllText(filePath);
                var movie = JsonSerializer.Deserialize<IEnumerable<Movie>>(json);

                ctx.Movies.AddRange(movie);

                ctx.SaveChanges();
            }

            if (!ctx.MovieRatings.Any())
            {
                //Then we need to create the sample data
                var filePath = Path.Combine(env.ContentRootPath, "Data/Default/rate.json");
                var json = File.ReadAllText(filePath);
                var rate = JsonSerializer.Deserialize<IEnumerable<MovieRating>>(json);

                ctx.MovieRatings.AddRange(rate);

                ctx.SaveChanges();
            }
        }
    }
}
