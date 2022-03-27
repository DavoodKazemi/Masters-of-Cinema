using MastersOfCinema.Data.Entities;
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

            //Seed users data
            if (!ctx.Users.Any())
            {
                var filePath = Path.Combine(env.ContentRootPath, "Data/Default/user.json");
                var userJson = File.ReadAllText(filePath);
                var users = JsonSerializer.Deserialize<IEnumerable<User>>(userJson);

                dynamic UsersDynamic = Newtonsoft.Json.JsonConvert.DeserializeObject(userJson);

                int i = 0;
                IdentityResult result;
                foreach (var item in users)
                {
                    string passwrd = UsersDynamic[i].Password;

                    result = await _userManager.CreateAsync(item, passwrd);
                    if (result != IdentityResult.Success)
                    {
                        throw new InvalidOperationException("Could not create new user in seeder");
                    }
                    i++;
                }
                ctx.SaveChanges();
            }

            /*User user = await _userManager.FindByEmailAsync("SacredCode1@gmail.com");
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
            }*/

            //Seed director table
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

            //Seed movie table
            if (!ctx.Movies.Any())
            {
                //Then we need to create the sample data
                var filePath = Path.Combine(env.ContentRootPath, "Data/Default/movie.json");
                var json = File.ReadAllText(filePath);
                var movie = JsonSerializer.Deserialize<IEnumerable<Movie>>(json);

                ctx.Movies.AddRange(movie);

                ctx.SaveChanges();
            }

            //Seed movie rating table
            if (!ctx.MovieRatings.Any())
            {
                //Then we need to create the sample data
                var filePath = Path.Combine(env.ContentRootPath, "Data/Default/rate.json");
                var json = File.ReadAllText(filePath);

                //rate = a list of movie rating, but the user field needs to be passed by foreach loop
                var rate = JsonSerializer.Deserialize<IEnumerable<MovieRating>>(json);

                //List all the users - Also, in order to prevent randomness, we order users
                List<User> userList = ctx.Users.OrderBy(x => x.UserName).ToList();

                //array = all data in rate.json file.
                //We will use the UserId field in "array" to pass User to the "rate"
                dynamic array = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

                //before: UserId = "1" or "2" or ...
                //after: UserId = real user Ids.
                foreach (var item in array)
                {
                    int simpleUserId = item.UserId - 1;
                    string realUserId = userList[simpleUserId].Id;
                    item.UserId = realUserId;
                }

                int i = 0;

                //pass real users to the rate, using real ids in array
                foreach (var item in rate)
                {
                    string id = array[i].UserId;
                    var User = ctx.Users.Where(i => i.Id == id).FirstOrDefault();
                    item.User = User;
                    i++;
                }

                ctx.MovieRatings.AddRange(rate);
                ctx.SaveChanges();
            }

            //Seed log table
            if (!ctx.MovieLogs.Any())
            {
                //Then we need to create the sample data
                var filePath = Path.Combine(env.ContentRootPath, "Data/Default/log.json");
                var json = File.ReadAllText(filePath);

                //log = a list of logs, but the user field needs to be passed by foreach loop
                var log = JsonSerializer.Deserialize<IEnumerable<MovieLog>>(json);

                //List all the users - Also, in order to prevent randomness, we order users
                List<User> userList = ctx.Users.OrderBy(x => x.UserName).ToList();

                //array = all data in log.json file.
                //We will use the UserId field in "array" to pass User to the "log"
                dynamic array = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

                //before: UserId = "1" or "2" or ...
                //after: UserId = real user Ids.
                foreach (var item in array)
                {
                    int simpleUserId = item.UserId - 1;
                    string realUserId = userList[simpleUserId].Id;
                    item.UserId = realUserId;
                }

                int i = 0;

                //pass real users to the log, using real ids in array
                foreach (var item in log)
                {
                    string id = array[i].UserId;
                    var User = ctx.Users.Where(i => i.Id == id).FirstOrDefault();
                    item.User = User;
                    i++;
                }

                ctx.MovieLogs.AddRange(log);
                ctx.SaveChanges();
            }

            //Seed watchlist table
            if (!ctx.Watchlists.Any())
            {
                //Then we need to create the sample data
                var filePath = Path.Combine(env.ContentRootPath, "Data/Default/watchlist.json");
                var json = File.ReadAllText(filePath);

                //watchlist = a list of watchilists, but the user field needs to be passed by foreach loop
                var watchlist = JsonSerializer.Deserialize<IEnumerable<Watchlist>>(json);

                //List all the users - Also, in order to prevent randomness, we order users
                List<User> userList = ctx.Users.OrderBy(x => x.UserName).ToList();

                //array = all data in watchlist.json file.
                //We will use the UserId field in "array" to pass User to the "watchlist"
                dynamic array = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

                //before: UserId = "1" or "2" or ...
                //after: UserId = real user Ids.
                foreach (var item in array)
                {
                    int simpleUserId = item.UserId - 1;
                    string realUserId = userList[simpleUserId].Id;
                    item.UserId = realUserId;
                }

                int i = 0;

                //pass real users to the watchlist, using real ids in array
                foreach (var item in watchlist)
                {
                    string id = array[i].UserId;
                    var User = ctx.Users.Where(i => i.Id == id).FirstOrDefault();
                    item.User = User;
                    i++;
                }

                ctx.Watchlists.AddRange(watchlist);
                ctx.SaveChanges();
            }

            //Seed custom lists table
            if (!ctx.Lists.Any())
            {
                //Then we need to create the sample data
                var filePath = Path.Combine(env.ContentRootPath, "Data/Default/custom-lists.json");
                var json = File.ReadAllText(filePath);

                //cLists = a list of custom lists, but the user field needs to be passed by foreach loop
                var cLists = JsonSerializer.Deserialize<IEnumerable<CList>>(json);

                //List all the users - Also, in order to prevent randomness, we order users
                List<User> userList = ctx.Users.OrderBy(x => x.UserName).ToList();

                //array = all data in custom-lists.json file.
                //We will use the UserId field in "array" to pass User to the "cLists"
                dynamic array = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

                //before: UserId = "1" or "2" or ...
                //after: UserId = real user Ids.
                foreach (var item in array)
                {
                    int simpleUserId = item.UserId - 1;
                    string realUserId = userList[simpleUserId].Id;
                    item.UserId = realUserId;
                }

                int i = 0;

                //pass real users to the cLists, using real ids in array
                foreach (var item in cLists)
                {
                    string id = array[i].UserId;
                    var User = ctx.Users.Where(i => i.Id == id).FirstOrDefault();
                    item.User = User;
                    i++;
                }

                ctx.Lists.AddRange(cLists);
                ctx.SaveChanges();
            }

            //Seed custom list movies table
            if (!ctx.ListMovies.Any())
            {
                //Then we need to create the sample data
                var filePath = Path.Combine(env.ContentRootPath, "Data/Default/custom-list-movie.json");
                var json = File.ReadAllText(filePath);
                var listMovies = JsonSerializer.Deserialize<IEnumerable<ListMovies>>(json);

                ctx.ListMovies.AddRange(listMovies);

                ctx.SaveChanges();
            }

            //Seed review table
            if (!ctx.Review.Any())
            {
                //Then we need to create the sample data
                var filePath = Path.Combine(env.ContentRootPath, "Data/Default/review.json");
                var json = File.ReadAllText(filePath);

                //reviews = a list of movie Review, but the user field needs to be passed by foreach loop
                var reviews = JsonSerializer.Deserialize<IEnumerable<Review>>(json);

                //List all the users - Also, in order to prevent randomness, we order users
                List<User> userList = ctx.Users.OrderBy(x => x.UserName).ToList();

                //array = all data in review.json file.
                //We will use the UserId field in "array" to pass User to the "reviews"
                dynamic array = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

                //before: UserId = "1" or "2" or ...
                //after: UserId = real user Ids.
                foreach (var item in array)
                {
                    int simpleUserId = item.UserId - 1;
                    string realUserId = userList[simpleUserId].Id;
                    item.UserId = realUserId;
                }

                int i = 0;

                //pass real users to the reviews, using real ids in array
                foreach (var item in reviews)
                {
                    string id = array[i].UserId;
                    var User = ctx.Users.Where(i => i.Id == id).FirstOrDefault();
                    item.User = User;
                    i++;
                }

                ctx.Review.AddRange(reviews);
                ctx.SaveChanges();
            }

            //Seed like review table
            if (!ctx.LikeReview.Any())
            {
                //Then we need to create the sample data
                var filePath = Path.Combine(env.ContentRootPath, "Data/Default/like-review.json");
                var json = File.ReadAllText(filePath);

                //likes = a list of LikeReview, but the user field needs to be passed by foreach loop
                var likes = JsonSerializer.Deserialize<IEnumerable<LikeReview>>(json);

                //List all the users - Also, in order to prevent randomness, we order users
                List<User> userList = ctx.Users.OrderBy(x => x.UserName).ToList();

                //array = all data in like-review.json file.
                //We will use the UserId field in "array" to pass User to the "likes"
                dynamic array = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

                //before: UserId = "1" or "2" or ...
                //after: UserId = real user Ids.
                foreach (var item in array)
                {
                    int simpleUserId = item.UserId - 1;
                    string realUserId = userList[simpleUserId].Id;
                    item.UserId = realUserId;
                }

                int i = 0;

                //pass real users to the likes, using real ids in array
                foreach (var item in likes)
                {
                    string id = array[i].UserId;
                    var User = ctx.Users.Where(i => i.Id == id).FirstOrDefault();
                    item.User = User;
                    i++;
                }

                ctx.LikeReview.AddRange(likes);
                ctx.SaveChanges();
            }
        }
    }
}
