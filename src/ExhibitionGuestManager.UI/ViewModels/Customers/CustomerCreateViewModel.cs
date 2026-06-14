using System.ComponentModel.DataAnnotations;

namespace ExhibitionGuestManager.UI.ViewModels.Customers;

public class CustomerCreateViewModel
{
    [Required(ErrorMessage = "TitleRequired")]
    [Display(Name = "Title")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "NameRequired")]
    [MaxLength(200)]
    [Display(Name = "Name")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(150)]
    [Display(Name = "Position")]
    public string? Position { get; set; }

    [Required(ErrorMessage = "MobileNumberRequired")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "MobileNumberMustBe10Digits")]
    [Display(Name = "MobileNumber")]
    public string MobileNumber { get; set; } = string.Empty;

    [MaxLength(100)]
    [Display(Name = "City")]
    public string? City { get; set; }

    [MaxLength(200)]
    [Display(Name = "OrganizationName")]
    public string? OrganizationName { get; set; }

    [EmailAddress(ErrorMessage = "EmailInvalid")]
    [MaxLength(256)]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [MinLength(1, ErrorMessage = "InterestRequired")]
    [Display(Name = "Interests")]
    public List<string> Interests { get; set; } = [];
}
