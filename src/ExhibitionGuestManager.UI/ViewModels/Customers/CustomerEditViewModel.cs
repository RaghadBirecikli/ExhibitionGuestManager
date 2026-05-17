using System.ComponentModel.DataAnnotations;
using ExhibitionGuestManager.Domain.Enums;

namespace ExhibitionGuestManager.UI.ViewModels.Customers;

public class CustomerEditViewModel
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    [Display(Name = "FullName")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    [Display(Name = "MobileNumber")]
    public string MobileNumber { get; set; } = string.Empty;

    [MaxLength(100)]
    [Display(Name = "City")]
    public string? City { get; set; }

    [MaxLength(200)]
    [Display(Name = "Company")]
    public string? CompanyName { get; set; }

    [MaxLength(150)]
    [Display(Name = "Department")]
    public string? Department { get; set; }

    [Display(Name = "Status")]
    public CustomerStatus Status { get; set; } = CustomerStatus.Active;

    [MaxLength(1000)]
    [Display(Name = "GeneralNotes")]
    public string? GeneralNotes { get; set; }

    [MaxLength(1000)]
    [Display(Name = "InternalNotes")]
    public string? InternalNotes { get; set; }
}
