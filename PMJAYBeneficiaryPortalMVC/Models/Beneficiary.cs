
using System.ComponentModel.DataAnnotations;

namespace PMJAYBeneficiaryPortalMVC.Models;

public class Beneficiary
{
    public int Id { get; set; }

    [Required]
    [StringLength(20)]
    [Display(Name = "Beneficiary ID")]
    public string BeneficiaryCode { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    [Display(Name = "Beneficiary Name")]
    public string BeneficiaryName { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Date of Birth")]
    public DateTime DateOfBirth { get; set; }

    [Required]
    [StringLength(10)]
    public string Gender { get; set; } = string.Empty;

    [StringLength(10)]
    [Display(Name = "Mobile Number")]
    public string? MobileNumber { get; set; }

    [StringLength(4)]
    [Display(Name = "Aadhaar Last 4 Digits")]
    public string? AadhaarLast4 { get; set; }

    [Required]
    [StringLength(50)]
    public string District { get; set; } = string.Empty;

    [StringLength(30)]
    [Display(Name = "Family ID")]
    public string? FamilyId { get; set; }

    [Display(Name = "Registration Date")]
    public DateTime RegistrationDate { get; set; } = DateTime.Now;

    // Beneficiary Workflow Status
    [Required]
    [StringLength(30)]
    public string Status { get; set; } = "VERIFICATION_PENDING";

    // Verification Details
    [StringLength(1000)]
    public string? VerificationRemarks { get; set; }

    public string? VerifiedBy { get; set; }

    public DateTime? VerifiedDate { get; set; }

    // Correction Details
    [StringLength(1000)]
    public string? CorrectionRemarks { get; set; }

    public string? CorrectionRequestedBy { get; set; }

    public DateTime? CorrectionRequestedDate { get; set; }

    public DateTime? ResubmittedDate { get; set; }

    // Final Admin Approval Details
    [StringLength(1000)]
    public string? ApprovalRemarks { get; set; }

    public string? ApprovedBy { get; set; }

    public DateTime? ApprovedDate { get; set; }
}
