using ExhibitionGuestManager.Domain.Enums;

namespace ExhibitionGuestManager.UI.ViewModels.Customers;

public class CustomerFilterViewModel
{
    public string? SearchTerm { get; set; }

    public string? City { get; set; }

    public string? CompanyName { get; set; }

    public string? Department { get; set; }

    public CustomerStatus? Status { get; set; }

    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}
