using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using e_Shop.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace e_Shop.Controllers
{
    [Route("role")]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public RolesController(RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [Route("rolelist")]
        public IActionResult RoleList() => View(_roleManager.Roles.ToList());

        [Route("create-new-role")]
        public IActionResult Create() => View();

        [HttpPost, Route("create-new-role")]
        public async Task<IActionResult> Create(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(name));
                if (result.Succeeded)
                    return RedirectToAction("RoleList");
            }
            return View(name);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var role = _roleManager.Roles.FirstOrDefault(r => r.Id == id);
            if (role != null)
                await _roleManager.DeleteAsync(role);

            return RedirectToAction("RoleList");
        }

        [Route("userlist")]
        public IActionResult UserList() => View(_userManager.Users.ToList());

        [Route("edit/{userId}")]
        public async Task<IActionResult> Edit(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.ToList();
            var model = new ChangeRoleViewModel()
            {
                UserId = user.Id.ToString(),
                UserEmail = user.Email,
                UserRoles = userRoles,
                Roles = allRoles
            };
            return View(model);
        }

        [HttpPost, Route("edit/{userId}")]
        public async Task<IActionResult> Edit(string userId, List<string> roles)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);
            var addedRoles = roles.Except(userRoles);
            var removedRoles = userRoles.Except(roles);

            await _userManager.AddToRolesAsync(user, addedRoles);
            await _userManager.RemoveFromRolesAsync(user, removedRoles);

            return RedirectToAction("Index", "Admin");
        }
    }
}