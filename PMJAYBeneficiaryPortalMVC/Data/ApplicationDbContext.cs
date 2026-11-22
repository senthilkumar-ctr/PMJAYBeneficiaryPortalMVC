
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PMJAYBeneficiaryPortalMVC.Models;

namespace PMJAYBeneficiaryPortalMVC.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Beneficiary> Beneficiaries { get; set; }
}
