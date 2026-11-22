
namespace PMJAYBeneficiaryPortalMVC.Models;

public class DatabaseSearchViewModel
{
    public string SearchNumber { get; set; } = "";

    public int FirstDatabaseCount { get; set; }
    public int SecondDatabaseCount { get; set; }

    public bool HasSearched { get; set; }
    public string? ErrorMessage { get; set; }

    public List<CMCHISMatchRow> CMCHISRecords { get; set; } = new();
    public List<PMJAYMatchRow> PMJAYRecords { get; set; } = new();
}

public class CMCHISMatchRow
{
    public string? URN { get; set; }
    public string? MemberName { get; set; }
    public string? TamilName { get; set; }
    public string? Gender { get; set; }
    public string? Mobile { get; set; }
    public string? RationCard { get; set; }
    public string? Address { get; set; }
    public string? Village { get; set; }
    public string? Taluk { get; set; }
    public string? District { get; set; }
    public string? RationShopCode { get; set; }
}

public class PMJAYMatchRow
{
    public string? ENR { get; set; }
    public string? BeneficiaryName { get; set; }
    public string? FatherSpouseName { get; set; }
    public string? Gender { get; set; }
    public string? Mobile { get; set; }
    public string? FamilyId { get; set; }
    public string? Address { get; set; }
    public string? District { get; set; }
    public string? Taluk { get; set; }
    public string? Village { get; set; }
    public string? ShopNumber { get; set; }
}
