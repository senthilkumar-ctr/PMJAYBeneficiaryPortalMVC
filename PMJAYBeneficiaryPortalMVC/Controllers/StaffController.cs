
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace PMJAYBeneficiaryPortalMVC.Controllers;

[Authorize(Roles = "Admin")]
public class StaffController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;

    public StaffController(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewBag.Roles = new List<string>
    {
        "Admin",
        "DataEntry",
        "Verification"
    };

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        string email,
        string password,
        string role)
    {
        ViewBag.Roles = new[] { "DataEntry", "Verification", "Admin" };

        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(role))
        {
            ViewBag.Error = "Please complete all fields.";
            return View();
        }

        var allowedRoles = new[]
   {
    "Admin",
    "DataEntry",
    "Verification"
};

        if (!allowedRoles.Contains(role))
        {
            ViewBag.Roles = allowedRoles;
            ViewBag.Error = "Invalid staff role.";
            return View();
        }
        email = email.Trim();

        var existingUser = await _userManager.FindByEmailAsync(email);

        if (existingUser != null)
        {
            ViewBag.Error = "This email address is already registered.";
            return View();
        }

        var user = new IdentityUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            var roleResult = await _userManager.AddToRoleAsync(user, role);

            if (roleResult.Succeeded)
            {
                TempData["Success"] = "Staff account created successfully.";
                return RedirectToAction("Create");
            }

            await _userManager.DeleteAsync(user);

            ViewBag.Error = string.Join(", ",
                roleResult.Errors.Select(e => e.Description));

            return View();
        }


        ViewBag.Error = string.Join(", ",
            result.Errors.Select(e => e.Description));

        return View();
    }


    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var users = _userManager.Users
            .OrderBy(u => u.Email)
            .ToList();

        var staffList = new List<StaffListItem>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            staffList.Add(new StaffListItem
            {
                Id = user.Id,
                Email = user.Email ?? "",
                Role = string.Join(", ", roles),
                IsActive = !user.LockoutEnd.HasValue ||
                           user.LockoutEnd.Value <= DateTimeOffset.UtcNow
            });
        }

        return View(staffList);
    }

    public class StaffListItem
    {
        public string Id { get; set; } = "";
        public string Email { get; set; } = "";
        public string Role { get; set; } = "";
        public bool IsActive { get; set; }
    }




    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
            return NotFound();

        // Prevent the Admin from disabling their own account.
        if (user.Id == _userManager.GetUserId(User))
        {
            TempData["Error"] = "You cannot deactivate your own account.";
            return RedirectToAction(nameof(Index));
        }

        var isActive = !user.LockoutEnd.HasValue ||
                       user.LockoutEnd.Value <= DateTimeOffset.UtcNow;

        if (isActive)
        {
            user.LockoutEnabled = true;
            user.LockoutEnd = DateTimeOffset.MaxValue;
        }
        else
        {
            user.LockoutEnd = null;
            user.LockoutEnabled = true;
        }

        var result = await _userManager.UpdateAsync(user);

        TempData[result.Succeeded ? "Success" : "Error"] =
            result.Succeeded
                ? (isActive ? "Staff account deactivated." : "Staff account activated.")
                : string.Join(", ", result.Errors.Select(e => e.Description));

        return RedirectToAction(nameof(Index));
    }



    [HttpGet]
    public async Task<IActionResult> EditRole(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
            return NotFound();

        if (user.Id == _userManager.GetUserId(User))
        {
            TempData["Error"] = "You cannot change your own role.";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Email = user.Email;
        ViewBag.UserId = user.Id;
        ViewBag.Roles = new[] { "Admin", "DataEntry", "Verification" };

        var currentRoles = await _userManager.GetRolesAsync(user);
        ViewBag.CurrentRole = currentRoles.FirstOrDefault() ?? "";

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditRole(string id, string role)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
            return NotFound();

        if (user.Id == _userManager.GetUserId(User))
        {
            TempData["Error"] = "You cannot change your own role.";
            return RedirectToAction(nameof(Index));
        }

        string[] allowedRoles = { "Admin", "DataEntry", "Verification" };

        if (!allowedRoles.Contains(role))
        {
            TempData["Error"] = "Invalid role selected.";
            return RedirectToAction(nameof(Index));
        }

        var currentRoles = await _userManager.GetRolesAsync(user);

        if (currentRoles.Contains("Admin") &&
            role != "Admin" &&
            _userManager.Users.Count() <= 1)
        {
            TempData["Error"] = "Cannot remove the last Admin account.";
            return RedirectToAction(nameof(Index));
        }

        var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);

        if (!removeResult.Succeeded)
        {
            TempData["Error"] = string.Join(", ",
                removeResult.Errors.Select(e => e.Description));
            return RedirectToAction(nameof(Index));
        }

        var addResult = await _userManager.AddToRoleAsync(user, role);

        if (!addResult.Succeeded)
        {
            TempData["Error"] = string.Join(", ",
                addResult.Errors.Select(e => e.Description));

            // Restore the previous roles if assignment fails.
            await _userManager.AddToRolesAsync(user, currentRoles);

            return RedirectToAction(nameof(Index));
        }

        TempData["Success"] = "Staff role updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> ResetPassword(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
            return NotFound();

        ViewBag.UserId = user.Id;
        ViewBag.Email = user.Email;

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(
        string id,
        string newPassword)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
            return NotFound();

        if (string.IsNullOrWhiteSpace(newPassword))
        {
            ViewBag.Error = "Please enter a new password.";
            ViewBag.UserId = user.Id;
            ViewBag.Email = user.Email;
            return View();
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var result = await _userManager.ResetPasswordAsync(
            user, token, newPassword);

        if (result.Succeeded)
        {
            TempData["Success"] = "Staff password reset successfully.";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Error = string.Join(", ",
            result.Errors.Select(e => e.Description));

        ViewBag.UserId = user.Id;
        ViewBag.Email = user.Email;

        return View();
    }

}
