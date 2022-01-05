using MastersOfCinema.Data;
using MastersOfCinema.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MastersOfCinema.Controllers
{
    public class DirectorController : Controller
    {
        private readonly Context _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public DirectorController(Context context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _hostEnvironment = webHostEnvironment;
        }

        // GET: Diretor
        public async Task<IActionResult> Index()
        {
            return View(await _context.Directors.ToListAsync());
        }

        // GET: Image/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Image/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Country,Bio,ImageFile")] Director directorViewModel)
        {
            if (ModelState.IsValid)
            {
                
                string fileName;

                if (directorViewModel.ImageFile == null)
                {
                    //If image not uploaded, assign the default photo
                    fileName = "DirectorsDefaultImage.jpg";
                    directorViewModel.ImageName = fileName;
                }
                else
                {
                    //If image uploaded, Save image to wwwroot/image
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    fileName = Path.GetFileNameWithoutExtension(directorViewModel.ImageFile.FileName);
                    string extension = Path.GetExtension(directorViewModel.ImageFile.FileName);
                    directorViewModel.ImageName = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    var path = Path.Combine(wwwRootPath + "/Image/", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await directorViewModel.ImageFile.CopyToAsync(fileStream);
                    }
                }
                
                //Insert record
                _context.Add(directorViewModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(directorViewModel);
        }

        // GET: Image/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var directors = await _context.Director.Include(x => x.Movies).ToListAsync();


            var imageModel = await _context.Directors.Include(x => x.Movies)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (imageModel == null)
            {
                return NotFound();
            }

            return View(imageModel);
        }

        // GET: Directors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {


            if (id == null)
            {
                return NotFound();
            }

            var directorViewModel = await _context.Directors.FindAsync(id);
            if (directorViewModel == null)
            {
                return NotFound();
            }
            return View(directorViewModel);
        }

        // POST: Directors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Country,Bio,ImageFile,ImageName")] Director directorViewModel)
        {
            var Director = await _context.Directors.FirstOrDefaultAsync(m => m.Id == id);
            var OriginalImage = Director.ImageName;

            if (id != directorViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (directorViewModel.ImageFile == null)
                    {
                        //If image not uploaded, assign the default photo
                        directorViewModel.ImageName = OriginalImage;
                    }
                    else
                    {
                        string fileName;
                        //If image uploaded, Save image to wwwroot/image
                        string wwwRootPath = _hostEnvironment.WebRootPath;
                        fileName = Path.GetFileNameWithoutExtension(directorViewModel.ImageFile.FileName);
                        string extension = Path.GetExtension(directorViewModel.ImageFile.FileName);
                        directorViewModel.ImageName = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                        var path = Path.Combine(wwwRootPath + "/Image/", fileName);
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await directorViewModel.ImageFile.CopyToAsync(fileStream);
                        }
                    }


                    var local = _context.Set<Director>()
                    .Local
                    .FirstOrDefault(entry => entry.Id.Equals(id));
                    // check if local is not null 
                    if (local != null)
                    {
                        // detach
                        _context.Entry(local).State = EntityState.Detached;
                    }

                    _context.Update(directorViewModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DirectorViewModelExists(directorViewModel.Id))
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
            return View(directorViewModel);
        }

        private bool DirectorViewModelExists(int id)
        {
            return _context.Directors.Any(e => e.Id == id);
        }
    }
}
