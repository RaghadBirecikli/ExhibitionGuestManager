using ExhibitionGuestManager.Application.Common;
using ExhibitionGuestManager.Application.DTOs;
using ExhibitionGuestManager.Application.Models;

namespace ExhibitionGuestManager.Application.Interfaces;

public interface ICustomerService
{
    Task<PaginatedResult<CustomerDto>> GetPagedAsync(CustomerFilterModel filter, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CustomerDto>> GetForExportAsync(CustomerFilterModel filter, CancellationToken cancellationToken = default);

    Task<CustomerDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<int> CreateAsync(CreateCustomerDto dto, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(UpdateCustomerDto dto, CancellationToken cancellationToken = default);

    Task<bool> SoftDeleteAsync(int id, CancellationToken cancellationToken = default);
}
