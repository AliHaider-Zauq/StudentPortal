using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentPortal.ViewModels;
using StudentPortal.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StudentPortal.ViewModel;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

public class AdminController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IActionResult> AllUsers()
    {
        var users = _userManager.Users.ToList();
        return View(users);
    }

    // GET: Edit User
    [HttpGet]
    [Authorize(Policy = "EditOtherUsersPolicy")]
    public async Task<IActionResult> EditUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        // Fetch user roles and claims
        var userRoles = await _userManager.GetRolesAsync(user);
        var userClaims = await _userManager.GetClaimsAsync(user);

        var model = new EditUserViewModel
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Roles = userRoles.Any() ? userRoles.ToList() : new List<string> { "None at the moment." },
            UserClaims = userClaims.Any() ? userClaims.Select(c => c.Type).ToList() : new List<string> { "None at the moment." }
        };

        return View(model);
    }


    [HttpPost]
    public async Task<IActionResult> EditUser(EditUserViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.FindByIdAsync(model.Id);
        if (user == null)
        {
            return NotFound();
        }

        // Check if email already exists
        var existingUser = await _userManager.FindByEmailAsync(model.Email);
        if (existingUser != null && existingUser.Id != model.Id)
        {
            ModelState.AddModelError("Email", "This email already exists.");
            return View(model);
        }

        // Update user details
        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.Email = model.Email;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }

        return RedirectToAction("AllUsers"); 
    }

    //DELETE USER
    [HttpPost]
    public async Task<IActionResult> DeleteUser(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Error deleting user.");
            return RedirectToAction("AllUsers");
        }

        return RedirectToAction("AllUsers");
    }
    public async Task<IActionResult> ManageRoles(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        ViewBag.UserId = user.Id;
        var roles = _roleManager.Roles.ToList();
        var userRoles = await _userManager.GetRolesAsync(user);

        var model = roles.Select(role => new ManageUsersRolesViewModel
        {
            RoleId = role.Id,
            RoleName = role.Name,
            IsSelected = userRoles.Contains(role.Name)
        }).ToList();

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> ManageRoles(string userId, List<ManageUsersRolesViewModel> model)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        var userRoles = await _userManager.GetRolesAsync(user);
        var selectedRoles = model.Where(r => r.IsSelected).Select(r => r.RoleName).ToList();

        // Remove unselected roles
        await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
        // Add newly selected roles
        await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

        return RedirectToAction("EditUser", new { id = userId });
      
    }

    // ✅ GET: Show Manage User Claims Page
    public async Task<IActionResult> ManageClaims(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        var userClaims = await _userManager.GetClaimsAsync(user);

        var model = new ManageUserClaimsViewModel
        {
            UserId = user.Id,
            Email = user.Email,
            Claims = UserClaims.AllClaims.Select(claim => new UserClaim
            {
                ClaimType = claim,
                IsSelected = userClaims.Any(uc => uc.Type == claim)
            }).ToList()
        };

        return View(model);
    }

    // ✅ POST: Update User Claims
    [HttpPost]
    public async Task<IActionResult> ManageClaims(ManageUserClaimsViewModel model)
    {
        var user = await _userManager.FindByIdAsync(model.UserId);
        if (user == null) return NotFound();

        var existingClaims = await _userManager.GetClaimsAsync(user);
        var selectedClaims = model.Claims.Where(c => c.IsSelected).Select(c => new Claim(c.ClaimType, "true")).ToList();

        // 🔹 Remove unselected claims
        foreach (var claim in existingClaims)
        {
            if (!selectedClaims.Any(sc => sc.Type == claim.Type))
            {
                await _userManager.RemoveClaimAsync(user, claim);
            }
        }

        // 🔹 Add newly selected claims
        foreach (var claim in selectedClaims)
        {
            if (!existingClaims.Any(ec => ec.Type == claim.Type))
            {
                await _userManager.AddClaimAsync(user, claim);
            }
        }

        return RedirectToAction("EditUser", new { id = model.UserId });
    }
}



