using ApplicationCore.Entities.Concrete;
using ApplicationCore.Entities.DTO_s.MovieDTO;
using ApplicationCore.Interfaces;
using AutoMapper;
using BilgeAdam_Final_Project.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BilgeAdam_Final_Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class MoviesController : Controller
    {
        private readonly IRepositoryService<Movie> _moviesRepo;
        private readonly IMapper _mapper;
        private readonly IMovieCategoryService _movieCategoriesRepo;
        private readonly IRepositoryService<Category> _categoriesRepo;
        private readonly IDirectorService _directorsRepo;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MoviesController(IRepositoryService<Movie> moviesRepo, IMapper mapper,IMovieCategoryService movieCategoriesRepo,IRepositoryService<Category> categoriesRepo,IDirectorService directorsRepo,IWebHostEnvironment webHostEnvironment)
        {
            _moviesRepo = moviesRepo;
            _mapper = mapper;
            _movieCategoriesRepo = movieCategoriesRepo;
            _categoriesRepo = categoriesRepo;
            _directorsRepo = directorsRepo;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            var movies = await _moviesRepo.GetFilteredListAsync
                (select: x => new GetMovieVM
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Year = x.Year,
                    Image = x.Image != null ? x.Image : "noimage.png",
                    DirectorName = x.Director.FirstName + " " + x.Director.LastName,
                    CreatedDate = x.CreatedDate,
                    UpdatedDate = x.UpdatedDate,
                    Status = x.Status,
                    Categories = _movieCategoriesRepo.GetStringCategoriesByMovieId(x.Id)
                },
                where: x => x.Status != ApplicationCore.Entities.Abstract.Status.Passive,
                orderBy: x => x.OrderByDescending(z => z.CreatedDate),
                join: x => x.Include(z => z.Director)//bu bağlantı olmazsa null gelir
                );

            
            return View(movies);
        }
        
        //jksdbka

        public async Task<IActionResult> AddMovie()
        {
            ViewBag.Categories = new SelectList
                (
                await _categoriesRepo.GetAllAsync(), "Id", "Name"
                );
            ViewBag.Directors = new SelectList
                (
                await _directorsRepo.GetDirectors(), "Id", "FullName"
                );
            return View();
               
        }

        [HttpPost]
        public async Task<IActionResult> AddMovie(AddMovieDTO model)
        {
            ViewBag.Categories = new SelectList
                (
                await _categoriesRepo.GetAllAsync(), "Id", "Name"
                );
            ViewBag.Directors = new SelectList
                (
                await _directorsRepo.GetDirectors(), "Id", "FullName"
                );
            if (ModelState.IsValid)
            {
                string imageName = "noimage.png";
                if (model.UploadImage !=null)
                {
                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "images/movies");
                    imageName=$"{Guid.NewGuid().ToString()}_{model.UploadImage.FileName}";
                    string filePath=Path.Combine(uploadDir, imageName);
                    FileStream fileStream = new FileStream(filePath, FileMode.Create);
                    await model.UploadImage.CopyToAsync(fileStream);
                    fileStream.Close();
                }
                var movie = _mapper.Map<Movie>(model);
                movie.Image = imageName;
                await _moviesRepo.AddAsync(movie);
                foreach(var item in model.Categories)
                {
                    var movieCategory = new MovieCategory()
                    {
                        MovieId = movie.Id,
                        CategoryId = Convert.ToInt32(item)
                    };
                    await _movieCategoriesRepo.AddMovieCategory(movieCategory);
                }
                TempData["Success"] = "film eklendi";
                return RedirectToAction("Index");

            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateMovie(int id)
        {
            var movie=await _moviesRepo.GetByIdAsync(id);
            if(movie != null)
            {
                var director = await _directorsRepo.GetByIdAsync(movie.DirectorId);
                ViewBag.Directors = new SelectList
                    (
                        await _directorsRepo.GetDirectors(), "Id", "FullName", director
                    );
                List<Category> hasCategories = new List<Category>();
                List<Category> hasNotCategories = new List<Category>();

                foreach(var item in await _categoriesRepo.GetAllAsync())
                {
                    if (_movieCategoriesRepo.GetAllByMovieId(movie.Id).Result.Any(x => x.CategoryId == item.Id))
                    {
                        hasCategories.Add(item);
                    }
                    else
                    {
                        hasNotCategories.Add(item);
                    }
                }
                var model = _mapper.Map<UpdateMovieDTO>(movie);
                model.AddCategories=hasCategories;
                model.DeleteCategories=hasNotCategories;
                return View(model);
            }
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> UpdateMovie(UpdateMovieDTO model)
        {
            ViewBag.Categories = new SelectList
               (
               await _categoriesRepo.GetAllAsync(), "Id", "Name"
               );
            ViewBag.Directors = new SelectList
                (
                await _directorsRepo.GetDirectors(), "Id", "FullName"
                );
            if (ModelState.IsValid)
            {
                string imageName = model.Image;//resim değiştirmesende resim gelsin
                if (model.UploadImage != null)
                {
                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "images/movies");
                    if (!string.Equals(model.Image, "noimage.png"))
                    {
                        string oldPath = Path.Combine(uploadDir, model.Image);
                        if (System.IO.File.Exists(oldPath))
                        {
                            System.IO.File.Delete(oldPath);
                        }
                    }
                    imageName = $"{Guid.NewGuid().ToString()}_{model.UploadImage.FileName}";
                    string filePath = Path.Combine(uploadDir, imageName);
                    FileStream fileStream = new FileStream(filePath, FileMode.Create);
                    await model.UploadImage.CopyToAsync(fileStream);
                    fileStream.Close();
                }
                foreach (var categoryId in model.AddIds ?? new string[] { })
                {
                    var movieCategory = new MovieCategory { MovieId = model.Id, CategoryId = Convert.ToInt32(categoryId) };
                    await _movieCategoriesRepo.AddMovieCategory(movieCategory);
                }

                foreach (var categoryId in model.DeleteIds ?? new string[] { })
                {
                    await _movieCategoriesRepo.DeleteMovieCategory(Convert.ToInt32(categoryId), model.Id);
                }

                var movie = _mapper.Map<Movie>(model);
                movie.Image = imageName;
                await _moviesRepo.UpdateAsync(movie);
                TempData["Success"] = "film güncellendi!!";
                return RedirectToAction("Index");
            }
            TempData["Error"] = "film güncellenemedi!!";
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            var movie=await _moviesRepo.GetByIdAsync(id);
            if(movie != null )
            {
                await _moviesRepo.DeleteAsync(movie);
                TempData["Success"] = "Film silindi";
                return RedirectToAction("Index");
            }
            TempData["Error"] = "Film silinmedi";
            return View();
        }















    }
}
