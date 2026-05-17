using ExhibitionGuestManager.Domain.Enums;

namespace ExhibitionGuestManager.Application.DTOs;

public class CreateCustomerDto
{
    public string FullName { get; set; } = string.Empty;

    public string MobileNumber { get; set; } = string.Empty;

    public string? City { get; set; }

    public string? CompanyName { get; set; }

    public string? Department { get; set; }

    public CustomerStatus Status { get; set; } = CustomerStatus.Active;

    public string? GeneralNotes { get; set; }

    public string? InternalNotes { get; set; }
}
