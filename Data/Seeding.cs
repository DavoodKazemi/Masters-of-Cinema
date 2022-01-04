using MastersOfCinema.ViewModels;
using Microsoft.AspNetCore.Hosting;
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

        public Seeding(Context ctx, IWebHostEnvironment env)
        {
            this.ctx = ctx;
            this.env = env;
        }

        public void Seed()
        {
            ctx.Database.EnsureCreated();

            if (!ctx.Directors.Any())
            {
                //Then we need to create the sample data
                var filePath = Path.Combine(env.ContentRootPath, "Data/director.json");
                var json = File.ReadAllText(filePath);
                var directors = JsonSerializer.Deserialize<IEnumerable<Director>>(json);

                ctx.Directors.AddRange(directors);


            /*var director = new DirectorViewModel()
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
                var filePath = Path.Combine(env.ContentRootPath, "Data/movie.json");
                var json = File.ReadAllText(filePath);
                var movie = JsonSerializer.Deserialize<IEnumerable<Movie>>(json);

                ctx.Movies.AddRange(movie);

                ctx.SaveChanges();
            }
        }
    }
}
