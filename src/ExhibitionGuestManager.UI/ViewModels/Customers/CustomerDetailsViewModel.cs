namespace ExhibitionGuestManager.UI.ViewModels.Customers;

public class CustomerDetailsViewModel
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Position { get; set; }

    public string MobileNumber { get; set; } = string.Empty;

    public string? City { get; set; }

    public string? OrganizationName { get; set; }

    public string? Email { get; set; }

    public IReadOnlyList<string> Interests { get; set; } = Array.Empty<string>();

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }
}
