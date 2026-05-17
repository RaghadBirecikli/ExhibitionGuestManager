using ExhibitionGuestManager.Domain.Enums;

namespace ExhibitionGuestManager.UI.ViewModels.Customers;

public class CustomerDetailsViewModel
{
    public int Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string MobileNumber { get; set; } = string.Empty;

    public string? City { get; set; }

    public string? CompanyName { get; set; }

    public string? Department { get; set; }

    public CustomerStatus Status { get; set; }

    public string? GeneralNotes { get; set; }

    public string? InternalNotes { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }
}
