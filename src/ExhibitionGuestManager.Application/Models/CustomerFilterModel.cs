namespace ExhibitionGuestManager.Application.Models;

public class CustomerFilterModel
{
    public string? SearchTerm { get; set; }

    public string? City { get; set; }

    public string? OrganizationName { get; set; }

    public string? Position { get; set; }

    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}
