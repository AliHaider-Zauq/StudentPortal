using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Models;
using StudentPortal.ViewModels;
using System.Threading.Tasks;

namespace StudentPortal.Controllers
{
    [Authorize(Roles = "Admin")] 
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public RoleController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        //Get List Of All Roles
        public IActionResult RoleList()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }

        // GET
        [HttpGet]
        [Authorize(Policy ="CanCreateRoles")]
        public IActionResult CreateRole()
        {
            return View(new RoleViewModel());
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole(RoleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // 🔹 Check if Role Already Exists
            var existingRole = await _roleManager.FindByNameAsync(model.RoleName);
            if (existingRole != null)
            {
                ModelState.AddModelError("RoleName", "This role already exists.");
                return View(model);
            }

            // 🔹 Create New Role
            var role = new IdentityRole(model.RoleName);
            var result = await _roleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                return RedirectToAction("RoleList", "Role");
            }

            // 🔹 Handle Errors
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        //Edit Roleof The Users
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            var users = await _userManager.GetUsersInRoleAsync(role.Name);
            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name,
                Users = users.Select(u => u.UserName).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditRoleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var role = await _roleManager.FindByIdAsync(model.Id);
            if (role == null)
            {
                return NotFound();
            }

            
            var existingRole = await _roleManager.FindByNameAsync(model.RoleName);
            if (existingRole != null && existingRole.Id != model.Id)
            {
                ModelState.AddModelError("RoleName", "This role name already exists.");
                return View(model);
            }

            // ✅ Update Role Name if it's unique
            role.Name = model.RoleName;
            var result = await _roleManager.UpdateAsync(role);

            if (result.Succeeded)
            {
                return RedirectToAction("RoleList", "Role"); 
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        public async Task<IActionResult> ManageUsers(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return NotFound();
            }

            var model = new ManageRoleUsersViewModel
            {
                RoleId = role.Id,
                RoleName = role.Name,
                Users = new List<UserRoleInfo>()
            };

            var users = await _userManager.Users.ToListAsync();

            foreach (var user in users)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var existingRole = userRoles.FirstOrDefault();

                model.Users.Add(new UserRoleInfo
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    IsSelected = await _userManager.IsInRoleAsync(user, role.Name),
                    ExistingRole = existingRole
                });
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageUsers(ManageRoleUsersViewModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.RoleId);
            if (role == null)
            {
                return NotFound();
            }

            foreach (var userInfo in model.Users)
            {
                var user = await _userManager.FindByIdAsync(userInfo.UserId);
                if (user != null)
                {
                    if (userInfo.IsSelected && !await _userManager.IsInRoleAsync(user, role.Name))
                    {
                        await _userManager.AddToRoleAsync(user, role.Name);
                    }
                    else if (!userInfo.IsSelected && await _userManager.IsInRoleAsync(user, role.Name))
                    {
                        await _userManager.RemoveFromRoleAsync(user, role.Name);
                    }
                }
            }

            return RedirectToAction("Edit", new { id = model.RoleId });
        }
        //DELETE USER
        [HttpPost]
        [Authorize(Policy ="CanDeleteRoles")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            var result = await _roleManager.DeleteAsync(role);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Error deleting role.");
                return RedirectToAction("RoleList");
            }

            return RedirectToAction("RoleList");
        }


    }

}

