using ExhibitionGuestManager.Application.Common;
using ExhibitionGuestManager.Application.DTOs;
using ExhibitionGuestManager.Application.Interfaces;
using ExhibitionGuestManager.Application.Models;
using ExhibitionGuestManager.Domain.Entities;
using ExhibitionGuestManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ExhibitionGuestManager.Infrastructure.Services;

public class CustomerService : ICustomerService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public CustomerService(ApplicationDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<PaginatedResult<CustomerDto>> GetPagedAsync(
        CustomerFilterModel filter,
        CancellationToken cancellationToken = default)
    {
        filter ??= new CustomerFilterModel();

        var pageNumber = Math.Max(1, filter.PageNumber);
        var pageSize = Math.Max(10, filter.PageSize);

        var query = ApplyFilters(_dbContext.Customers.AsNoTracking(), filter)
            .OrderByDescending(customer => customer.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var customers = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(customer => MapToDto(customer))
            .ToListAsync(cancellationToken);

        return new PaginatedResult<CustomerDto>(customers, totalCount, pageNumber, pageSize);
    }

    public async Task<IReadOnlyList<CustomerDto>> GetForExportAsync(
        CustomerFilterModel filter,
        CancellationToken cancellationToken = default)
    {
        filter ??= new CustomerFilterModel();

        return await ApplyFilters(_dbContext.Customers.AsNoTracking(), filter)
            .OrderByDescending(customer => customer.CreatedAt)
            .Select(customer => MapToDto(customer))
            .ToListAsync(cancellationToken);
    }

    public async Task<CustomerDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Customers
            .AsNoTracking()
            .Where(customer => customer.Id == id)
            .Select(customer => MapToDto(customer))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> CreateAsync(CreateCustomerDto dto, CancellationToken cancellationToken = default)
    {
        var customer = new Customer
        {
            FullName = dto.FullName,
            MobileNumber = dto.MobileNumber,
            City = dto.City,
            CompanyName = dto.CompanyName,
            Department = dto.Department,
            Status = dto.Status,
            GeneralNotes = dto.GeneralNotes,
            InternalNotes = dto.InternalNotes,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = GetCurrentUserIdentifier()
        };

        _dbContext.Customers.Add(customer);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return customer.Id;
    }

    public async Task<bool> UpdateAsync(UpdateCustomerDto dto, CancellationToken cancellationToken = default)
    {
        var customer = await _dbContext.Customers
            .FirstOrDefaultAsync(existingCustomer => existingCustomer.Id == dto.Id, cancellationToken);

        if (customer is null)
        {
            return false;
        }

        customer.FullName = dto.FullName;
        customer.MobileNumber = dto.MobileNumber;
        customer.City = dto.City;
        customer.CompanyName = dto.CompanyName;
        customer.Department = dto.Department;
        customer.Status = dto.Status;
        customer.GeneralNotes = dto.GeneralNotes;
        customer.InternalNotes = dto.InternalNotes;
        customer.UpdatedAt = DateTime.UtcNow;
        customer.UpdatedBy = GetCurrentUserIdentifier();

        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> SoftDeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var customer = await _dbContext.Customers
            .FirstOrDefaultAsync(existingCustomer => existingCustomer.Id == id, cancellationToken);

        if (customer is null)
        {
            return false;
        }

        customer.IsDeleted = true;
        customer.DeletedAt = DateTime.UtcNow;
        customer.DeletedBy = GetCurrentUserIdentifier();

        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static IQueryable<Customer> ApplyFilters(IQueryable<Customer> query, CustomerFilterModel filter)
    {
        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var searchTerm = filter.SearchTerm.Trim();
            query = query.Where(customer =>
                customer.FullName.Contains(searchTerm)
                || customer.MobileNumber.Contains(searchTerm)
                || (customer.City != null && customer.City.Contains(searchTerm))
                || (customer.CompanyName != null && customer.CompanyName.Contains(searchTerm))
                || (customer.Department != null && customer.Department.Contains(searchTerm)));
        }

        if (!string.IsNullOrWhiteSpace(filter.City))
        {
            var city = filter.City.Trim();
            query = query.Where(customer => customer.City != null && customer.City.Contains(city));
        }

        if (!string.IsNullOrWhiteSpace(filter.CompanyName))
        {
            var companyName = filter.CompanyName.Trim();
            query = query.Where(customer => customer.CompanyName != null && customer.CompanyName.Contains(companyName));
        }

        if (!string.IsNullOrWhiteSpace(filter.Department))
        {
            var department = filter.Department.Trim();
            query = query.Where(customer => customer.Department != null && customer.Department.Contains(department));
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(customer => customer.Status == filter.Status.Value);
        }

        if (filter.FromDate.HasValue)
        {
            query = query.Where(customer => customer.CreatedAt >= filter.FromDate.Value.Date);
        }

        if (filter.ToDate.HasValue)
        {
            query = query.Where(customer => customer.CreatedAt < filter.ToDate.Value.Date.AddDays(1));
        }

        return query;
    }

    private static CustomerDto MapToDto(Customer customer)
    {
        return new CustomerDto
        {
            Id = customer.Id,
            FullName = customer.FullName,
            MobileNumber = customer.MobileNumber,
            City = customer.City,
            CompanyName = customer.CompanyName,
            Department = customer.Department,
            Status = customer.Status,
            GeneralNotes = customer.GeneralNotes,
            InternalNotes = customer.InternalNotes,
            CreatedAt = customer.CreatedAt,
            CreatedBy = customer.CreatedBy,
            UpdatedAt = customer.UpdatedAt,
            UpdatedBy = customer.UpdatedBy
        };
    }

    private string? GetCurrentUserIdentifier()
    {
        return _currentUserService.UserId ?? _currentUserService.UserName;
    }
}
