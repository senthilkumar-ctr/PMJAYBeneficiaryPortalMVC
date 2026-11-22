
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PMJAYBeneficiaryPortalMVC.Data;
using PMJAYBeneficiaryPortalMVC.Models;

namespace PMJAYBeneficiaryPortalMVC.Controllers;

[Authorize]
public class BeneficiaryController : Controller
{
    private readonly ApplicationDbContext _context;

    public BeneficiaryController(ApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<IActionResult> Index(
        string? search,
        string? district,
        string? status,
        int page = 1)
    {
        const int pageSize = 10;

        var query = _context.Beneficiaries.AsNoTracking();

        // Search
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                x.BeneficiaryCode.Contains(search) ||
                x.BeneficiaryName.Contains(search) ||
                (x.MobileNumber != null &&
                 x.MobileNumber.Contains(search)));
        }

        // District filter
        if (!string.IsNullOrWhiteSpace(district))
        {
            query = query.Where(x => x.District == district);
        }

        // Status filter
        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(x => x.Status == status);
        }

        // Dropdown values
        ViewBag.Districts = await _context.Beneficiaries
            .AsNoTracking()
            .Select(x => x.District)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync();

        ViewBag.Search = search;
        ViewBag.SelectedDistrict = district;
        ViewBag.SelectedStatus = status;

        // Total records
        int totalRecords = await query.CountAsync();

        int totalPages = (int)Math.Ceiling(
            totalRecords / (double)pageSize);

        page = Math.Max(1, Math.Min(page, Math.Max(1, totalPages)));

        // Pagination
        var records = await query
            .OrderByDescending(x => x.RegistrationDate)
            .ThenByDescending(x => x.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.TotalRecords = totalRecords;
        ViewBag.PageSize = pageSize;

        return View(records);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Beneficiary beneficiary)
    {
        if (await _context.Beneficiaries.AnyAsync(
            x => x.BeneficiaryCode == beneficiary.BeneficiaryCode))
        {
            ModelState.AddModelError(
                nameof(beneficiary.BeneficiaryCode),
                "Beneficiary ID already exists.");
        }

        if (!ModelState.IsValid)
            return View(beneficiary);

        beneficiary.RegistrationDate = DateTime.Now;
        beneficiary.Status = "Pending";

        _context.Beneficiaries.Add(beneficiary);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Beneficiary registered successfully.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var beneficiary = await _context.Beneficiaries.FindAsync(id);

        if (beneficiary == null)
            return NotFound();

        return View(beneficiary);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var beneficiary = await _context.Beneficiaries.FindAsync(id);

        if (beneficiary == null)
            return NotFound();

        return View(beneficiary);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Beneficiary beneficiary)
    {
        if (id != beneficiary.Id)
            return NotFound();

        if (await _context.Beneficiaries.AnyAsync(
            x => x.BeneficiaryCode == beneficiary.BeneficiaryCode &&
                 x.Id != beneficiary.Id))
        {
            ModelState.AddModelError(
                nameof(beneficiary.BeneficiaryCode),
                "Beneficiary ID already exists.");
        }

        if (!ModelState.IsValid)
            return View(beneficiary);

        var existing = await _context.Beneficiaries.FindAsync(id);

        if (existing == null)
            return NotFound();

        existing.BeneficiaryCode = beneficiary.BeneficiaryCode;
        existing.BeneficiaryName = beneficiary.BeneficiaryName;
        existing.DateOfBirth = beneficiary.DateOfBirth;
        existing.Gender = beneficiary.Gender;
        existing.MobileNumber = beneficiary.MobileNumber;
        existing.AadhaarLast4 = beneficiary.AadhaarLast4;
        existing.District = beneficiary.District;
        existing.FamilyId = beneficiary.FamilyId;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Beneficiary updated successfully.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var beneficiary = await _context.Beneficiaries.FindAsync(id);

        if (beneficiary == null)
            return NotFound();

        return View(beneficiary);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var beneficiary = await _context.Beneficiaries.FindAsync(id);

        if (beneficiary == null)
            return NotFound();

        _context.Beneficiaries.Remove(beneficiary);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Beneficiary deleted successfully.";

        return RedirectToAction(nameof(Index));
    }
}


