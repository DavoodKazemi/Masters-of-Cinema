using MastersOfCinema.Data;
using MastersOfCinema.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MastersOfCinema.Controllers
{
    public class MovieController : Controller
    {
        private readonly Context _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public IList<Director> DirectorList { get; set; }
        private readonly ILogger<MovieController> logger;


        public MovieController(Context context, IWebHostEnvironment webHostEnvironment, ILogger<MovieController> logger)
        {
            _context = context;
            _hostEnvironment = webHostEnvironment;
            this.logger = logger;
        }

        private IEnumerable<Movie> GetMovies()
        {
            try
            {
                logger.LogInformation("GetMovies was called!");
                return _context.Movies.OrderBy(p => p.Id).ToList();

            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to get all movies: {ex}");
                return null;
            }

        }

        public IEnumerable<Director> GetDirectors()
        {
            try
            {
                logger.LogInformation("GetDirectors was called!");
                return _context.Directors.OrderBy(p => p.Id).ToList();

            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to get all directors: {ex}");
                return null;
            }

        }

        // GET: Movies
        public ActionResult Index()
        {
            var tupleModel = new Tuple<IEnumerable<Movie>, IEnumerable<Director>>(GetMovies(), GetDirectors());
            return View(tupleModel);

        }


        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //Bind director thing in order to display the IDs and Nemes of directors in the view
            List<Director> cl = new List<Director>();
            cl = (from c in _context.Directors select c).ToList();
            ViewBag.message = cl;
            //END Bind director thing

            var movieViewModel = await _context.Movies.FindAsync(id);
            if (movieViewModel == null)
            {
                return NotFound();
            }
            return View(movieViewModel);
        }


        
        // POST: Directors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Year,Description,ImageFile,ImageName,DirectorViewModelId")] Movie movieViewModel)
        {
            /*var OriginalImage = Movie.ImageName;*/

            var Movie = await _context.Movies.FirstOrDefaultAsync(m => m.Id == id);
            var OriginalImage = Movie.ImageName;
            //movieViewModel.DirectorViewModelId = 1;


            if (id != movieViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (movieViewModel.ImageFile == null)
                    {
                        //If image not uploaded, assign the default photo
                        movieViewModel.ImageName = Movie.ImageName;
                    }
                    else
                    {
                        //If image uploaded, Save image to wwwroot/image
                        string fileName;
                        //If image uploaded, Save image to wwwroot/image
                        string wwwRootPath = _hostEnvironment.WebRootPath;
                        fileName = Path.GetFileNameWithoutExtension(movieViewModel.ImageFile.FileName);
                        string extension = Path.GetExtension(movieViewModel.ImageFile.FileName);
                        movieViewModel.ImageName = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                        var path = Path.Combine(wwwRootPath + "/Image/", fileName);
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await movieViewModel.ImageFile.CopyToAsync(fileStream);
                        }
                    }

                    var local = _context.Set<Movie>()
                    .Local
                    .FirstOrDefault(entry => entry.Id.Equals(id));
                    // check if local is not null 
                    if (local != null)
                    {
                        // detach
                        _context.Entry(local).State = EntityState.Detached;
                    }

                    _context.Update(movieViewModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieViewModelExists(movieViewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(movieViewModel);
        }
        private bool MovieViewModelExists(int id)
        {
            return _context.Directors.Any(e => e.Id == id);
        }

        // GET: Movie/Create
        public IActionResult Create()
        {
/*            var tupleModel = new Tuple<IEnumerable<MovieViewModel>, IEnumerable<DirectorViewModel>>(GetMovies(), GetDirectors());
*/

            //Bind director thing in order to display the IDs and Nemes of directors in the view
            List<Director> cl = new List<Director>();
            cl = (from c in _context.Directors select c).ToList();
            ViewBag.message = cl;
            //END Bind director thing

            return View();
        }

        // POST: Image/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Year,Description,ImageFile,ImageName,DirectorViewModelId")] Movie movieViewModel)
        {
            if (ModelState.IsValid)
            {
                string fileName;

                if (movieViewModel.ImageFile == null)
                {
                    //If image not uploaded, assign the default photo
                    fileName = "DirectorsDefaultImage.jpg";
                    movieViewModel.ImageName = fileName;
                }
                else
                {
                    //If image uploaded, Save image to wwwroot/image
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    fileName = Path.GetFileNameWithoutExtension(movieViewModel.ImageFile.FileName);
                    string extension = Path.GetExtension(movieViewModel.ImageFile.FileName);
                    movieViewModel.ImageName = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    var path = Path.Combine(wwwRootPath + "/Image/", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await movieViewModel.ImageFile.CopyToAsync(fileStream);
                    }
                }

                //Insert record
                _context.Add(movieViewModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movieViewModel);
        }
    }
}
