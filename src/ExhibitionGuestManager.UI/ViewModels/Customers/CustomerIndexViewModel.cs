namespace ExhibitionGuestManager.UI.ViewModels.Customers;

public class CustomerIndexViewModel
{
    public IReadOnlyList<CustomerListItemViewModel> Customers { get; set; } = Array.Empty<CustomerListItemViewModel>();

    public CustomerFilterViewModel Filter { get; set; } = new();

    public int TotalCount { get; set; }

    public int TotalPages { get; set; }

    public bool HasPreviousPage { get; set; }

    public bool HasNextPage { get; set; }
}
