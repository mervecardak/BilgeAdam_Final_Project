using ApplicationCore.Entities.Concrete;
using ApplicationCore.Entities.DTO_s.DirectorDTO;
using ApplicationCore.Interfaces;
using AutoMapper;
using BilgeAdam_Final_Project.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace BilgeAdam_Final_Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class DirectorsController : Controller
    {
        private readonly IRepositoryService<Director> _directorRepo;
        private readonly IMapper _mapper;

        public DirectorsController(IRepositoryService<Director> directorRepo, IMapper mapper)
        {
            _directorRepo = directorRepo;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index()
        {
            var directors = await _directorRepo.GetFilteredListAsync
                (
                    select: x => new GetDirectorVM
                    {
                        Id = x.Id,
                        FullName = x.FirstName + " " + x.LastName,
                        BirthDate = x.BirthDate,
                        CreatedDate = x.CreatedDate,
                        Status = x.Status,
                        UpdatedDate = x.UpdatedDate
                    },
                    where: x => x.Status != ApplicationCore.Entities.Abstract.Status.Passive,
                    orderBy: x => x.OrderByDescending(z => z.CreatedDate)
                );

            return View(directors);
        }

        [HttpGet]
        public IActionResult AddDirector() => View();

        [HttpPost]
        public async Task<IActionResult> AddDirector(AddDirectorDTO model)
        {
            if (ModelState.IsValid) 
            {
                var director = _mapper.Map<Director>(model);
                await _directorRepo.AddAsync(director);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateDirector(int id)
        {
            var director = await _directorRepo.GetByIdAsync(id);
            if (director != null)
            {
                var model = _mapper.Map<UpdateDirectorDTO>(director);
                return View(model);
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDirector(UpdateDirectorDTO model)
        {
            if (ModelState.IsValid) 
            {
                var director = _mapper.Map<Director>(model);
                await _directorRepo.UpdateAsync(director);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteDirector(int id)
        {
            var director = await _directorRepo.GetByIdAsync(id);
            if (director != null)
            {
                await _directorRepo.DeleteAsync(director);
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
