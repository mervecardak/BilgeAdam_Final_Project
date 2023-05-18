using ApplicationCore.Entities.Concrete;
using ApplicationCore.Interfaces;
using BilgeAdam_Final_Project.Areas.Admin.Models;
using BilgeAdam_Final_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BilgeAdam_Final_Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IRepositoryService<Movie> _moviesRepo;
        private readonly IMovieCategoryService _movieCategoriesRepo;
        private readonly IDirectorService _diretorsRepo;
        private readonly IMovieCategoryService _movieCategoryRepo;

        public HomeController(ILogger<HomeController> logger,IRepositoryService<Movie> moviesRepo,IMovieCategoryService movieCategoriesRepo,IDirectorService diretorsRepo)
        {
            _logger = logger;
            _moviesRepo = moviesRepo;
            _movieCategoriesRepo = movieCategoriesRepo;
            _diretorsRepo = diretorsRepo;
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
        public async Task<IActionResult> MovieDetail(int id)
        {
            var movie = await _moviesRepo.GetByIdAsync(id);
            var director= await _diretorsRepo.GetByIdAsync(movie.DirectorId);
            var model = new GetMovieDetailVM
            {
                Id = movie.Id,
                Name = movie.Name,
                Description = movie.Description,
                Year = movie.Year,
                Image = movie.Image != null ? movie.Image : "noimage.png",
                DirectorName = director.FirstName + " " + director.LastName,
               
                Categories = _movieCategoriesRepo.GetStringCategoriesByMovieId(movie.Id)
            };
            return View(model);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}