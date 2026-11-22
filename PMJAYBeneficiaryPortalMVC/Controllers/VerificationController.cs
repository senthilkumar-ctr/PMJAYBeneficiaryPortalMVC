
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PMJAYBeneficiaryPortalMVC.Data;
using PMJAYBeneficiaryPortalMVC.Models;

namespace PMJAYBeneficiaryPortalMVC.Controllers;

[Authorize(Roles = "Verification,Admin")]
public class VerificationController : Controller
{
    private readonly ApplicationDbContext _context;

    public VerificationController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Verification pending list
    public async Task<IActionResult> Index()
    {
        var records = await _context.Beneficiaries
            .AsNoTracking()
            .Where(x => x.Status == "VERIFICATION_PENDING")
            .OrderBy(x => x.RegistrationDate)
            .ToListAsync();

        return View(records);
    }

    // View beneficiary
    public async Task<IActionResult> Details(int id)
    {
        var beneficiary = await _context.Beneficiaries
            .FirstOrDefaultAsync(x => x.Id == id);

        if (beneficiary == null)
            return NotFound();

        return View(beneficiary);
    }

    // Verify beneficiary
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Verify(int id, string? remarks)
    {
        var beneficiary = await _context.Beneficiaries.FindAsync(id);

        if (beneficiary == null)
            return NotFound();

        if (beneficiary.Status != "VERIFICATION_PENDING")
        {
            TempData["Error"] = "This record is not awaiting verification.";
            return RedirectToAction(nameof(Index));
        }

        beneficiary.Status = "VERIFIED";
        beneficiary.VerificationRemarks = remarks;
        beneficiary.VerifiedBy = User.Identity?.Name;
        beneficiary.VerifiedDate = DateTime.Now;

        await _context.SaveChangesAsync();

        TempData["Success"] = "Beneficiary verified successfully.";

        return RedirectToAction(nameof(Index));
    }

    // Return beneficiary for correction
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ReturnForCorrection(
        int id,
        string correctionRemarks)
    {
        if (string.IsNullOrWhiteSpace(correctionRemarks))
        {
            TempData["Error"] = "Correction remarks are required.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var beneficiary = await _context.Beneficiaries.FindAsync(id);

        if (beneficiary == null)
            return NotFound();

        if (beneficiary.Status != "VERIFICATION_PENDING")
        {
            TempData["Error"] = "This record is not awaiting verification.";
            return RedirectToAction(nameof(Index));
        }

        beneficiary.Status = "UPDATE_PENDING";
        beneficiary.CorrectionRemarks = correctionRemarks.Trim();
        beneficiary.CorrectionRequestedBy = User.Identity?.Name;
        beneficiary.CorrectionRequestedDate = DateTime.Now;

        await _context.SaveChangesAsync();

        TempData["Success"] = "Beneficiary returned for correction.";

        return RedirectToAction(nameof(Index));
    }

    // Admin final approval
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id, string? remarks)
    {
        var beneficiary = await _context.Beneficiaries.FindAsync(id);

        if (beneficiary == null)
            return NotFound();

        if (beneficiary.Status != "VERIFIED")
        {
            TempData["Error"] = "Only verified beneficiaries can be approved.";
            return RedirectToAction(nameof(Index));
        }

        beneficiary.Status = "APPROVED";
        beneficiary.ApprovalRemarks = remarks;
        beneficiary.ApprovedBy = User.Identity?.Name;
        beneficiary.ApprovedDate = DateTime.Now;

        await _context.SaveChangesAsync();

        TempData["Success"] = "Beneficiary approved successfully.";

        return RedirectToAction(nameof(Index));
    }
}
