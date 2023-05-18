using ApplicationCore.Entities.Concrete;
using ApplicationCore.Entities.DTO_s.RolesDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NuGet.Protocol.Core.Types;

namespace BilgeAdam_Final_Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="admin")]
    public class RolesController : Controller
    {
        
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }

        public IActionResult CreateRole()
        {
            return View();
        }
        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole(RoleDTO model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole role = new IdentityRole(model.Name);
                IdentityResult result = await _roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    TempData["Success"] = "Role kaydedildi.";
                    return RedirectToAction("Index");
                }
                foreach(IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                    TempData["error"] = error.Description;
                }
            }
            TempData["error"] = "Bişeyler ters gitti";
            return View(model);

        }
        public async Task<IActionResult> AssignedUser(string id)
        {
            IdentityRole role = await _roleManager.FindByIdAsync(id);
            List<ApplicationUser> HasRole=new List<ApplicationUser>();
            List<ApplicationUser> HasNotRole=new List<ApplicationUser>();
            foreach(ApplicationUser user in _userManager.Users)
            {
                var list = await _userManager.IsInRoleAsync(user, role.Name) ? HasRole : HasNotRole;
                list.Add(user);
            }
            AssignedRoleDTO model = new AssignedRoleDTO
            {
                Role = role,
                HasNotRole = HasNotRole,
                HasRole = HasRole,
                RoleName = role.Name
            };
            return View(model);
        }
        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignedUser(AssignedRoleDTO model)
        {
            IdentityResult result=new IdentityResult();
            foreach(var userId in model.AddIds ?? new string[] {})
            {
                ApplicationUser user= await _userManager.FindByIdAsync(userId);
                result = await _userManager.AddToRoleAsync(user, model.RoleName);
            }
            foreach(var userId in model.DeleteIds ?? new string[] { })
            {
                ApplicationUser user =await _userManager.FindByIdAsync(userId);
                result =await _userManager.RemoveFromRoleAsync(user,model.RoleName);
            }
            if(result.Succeeded )
            {
                TempData["Success"] = "Kullanıcılar role başarılı bir şekilde atandı";
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public async Task<IActionResult> RemoveRole(string id) 
        { 
            var role=await _roleManager.FindByIdAsync(id);
            IdentityResult result=await _roleManager.DeleteAsync(role);
            if(result.Succeeded)
            {
                TempData["Success"] = "Role Silindi";
            }
            foreach(IdentityError error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
                TempData["Error"] = error.Description;
            }
            return RedirectToAction("Index");
        }












    }
}
