using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PMJAYBeneficiaryPortalMVC.Data;

namespace PMJAYBeneficiaryPortalMVC.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.TotalBeneficiaries =
            await _context.Beneficiaries.CountAsync();

        ViewBag.PendingCount =
            await _context.Beneficiaries.CountAsync(x => x.Status == "Pending");

        ViewBag.ApprovedCount =
            await _context.Beneficiaries.CountAsync(x => x.Status == "Approved");

        ViewBag.RejectedCount =
            await _context.Beneficiaries.CountAsync(x => x.Status == "Rejected");

        ViewBag.DistrictSummary = await _context.Beneficiaries
            .GroupBy(x => x.District)
            .Select(g => new
            {
                District = g.Key,
                Total = g.Count()
            })
            .OrderByDescending(x => x.Total)
            .ToListAsync();

        return View();
    }
}