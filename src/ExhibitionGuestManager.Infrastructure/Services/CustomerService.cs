using ExhibitionGuestManager.Application.Common;
using ExhibitionGuestManager.Application.DTOs;
using ExhibitionGuestManager.Application.Interfaces;
using ExhibitionGuestManager.Application.Models;
using ExhibitionGuestManager.Domain.Entities;
using ExhibitionGuestManager.Infrastructure.Persistence;
using System.Text.Json;
using ExhibitionGuestManager.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ExhibitionGuestManager.Infrastructure.Services;

public class CustomerService : ICustomerService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

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

        var customers = await ApplyFilters(_dbContext.Customers.AsNoTracking(), filter)
            .OrderByDescending(customer => customer.CreatedAt)
            .Select(customer => MapToDto(customer))
            .ToListAsync(cancellationToken);

        await ResolveAuditUsersAsync(customers, cancellationToken);

        return customers;
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
            FullName = dto.Name,
            MobileNumber = dto.MobileNumber,
            City = dto.City,
            CompanyName = dto.OrganizationName,
            Department = dto.Position,
            Status = CustomerStatus.Active,
            GeneralNotes = dto.Email,
            InternalNotes = SerializeMetadata(dto.Title, dto.Interests),
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

        customer.FullName = dto.Name;
        customer.MobileNumber = dto.MobileNumber;
        customer.City = dto.City;
        customer.CompanyName = dto.OrganizationName;
        customer.Department = dto.Position;
        customer.Status = CustomerStatus.Active;
        customer.GeneralNotes = dto.Email;
        customer.InternalNotes = SerializeMetadata(dto.Title, dto.Interests);
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
                || (customer.Department != null && customer.Department.Contains(searchTerm))
                || (customer.GeneralNotes != null && customer.GeneralNotes.Contains(searchTerm))
                || (customer.InternalNotes != null && customer.InternalNotes.Contains(searchTerm)));
        }

        if (!string.IsNullOrWhiteSpace(filter.City))
        {
            var city = filter.City.Trim();
            query = query.Where(customer => customer.City != null && customer.City.Contains(city));
        }

        if (!string.IsNullOrWhiteSpace(filter.OrganizationName))
        {
            var organizationName = filter.OrganizationName.Trim();
            query = query.Where(customer => customer.CompanyName != null && customer.CompanyName.Contains(organizationName));
        }

        if (!string.IsNullOrWhiteSpace(filter.Position))
        {
            var position = filter.Position.Trim();
            query = query.Where(customer => customer.Department != null && customer.Department.Contains(position));
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
        var metadata = DeserializeMetadata(customer.InternalNotes);

        return new CustomerDto
        {
            Id = customer.Id,
            Title = metadata.Title,
            Name = customer.FullName,
            Position = customer.Department,
            MobileNumber = customer.MobileNumber,
            City = customer.City,
            OrganizationName = customer.CompanyName,
            Email = customer.GeneralNotes,
            Interests = metadata.Interests,
            CreatedAt = customer.CreatedAt,
            CreatedBy = customer.CreatedBy,
            UpdatedAt = customer.UpdatedAt,
            UpdatedBy = customer.UpdatedBy
        };
    }

    private static string SerializeMetadata(string title, IReadOnlyList<string> interests)
    {
        var metadata = new CustomerMetadata(title, interests.Where(interest => !string.IsNullOrWhiteSpace(interest)).Distinct().ToArray());
        return JsonSerializer.Serialize(metadata, JsonOptions);
    }

    private static CustomerMetadata DeserializeMetadata(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new CustomerMetadata(string.Empty, Array.Empty<string>());
        }

        try
        {
            var metadata = JsonSerializer.Deserialize<CustomerMetadata>(value, JsonOptions);
            return metadata is null
                ? new CustomerMetadata(string.Empty, Array.Empty<string>())
                : new CustomerMetadata(metadata.Title ?? string.Empty, metadata.Interests ?? Array.Empty<string>());
        }
        catch (JsonException)
        {
            return new CustomerMetadata(string.Empty, Array.Empty<string>());
        }
    }

    private async Task ResolveAuditUsersAsync(List<CustomerDto> customers, CancellationToken cancellationToken)
    {
        var auditValues = customers
            .SelectMany(customer => new[] { customer.CreatedBy, customer.UpdatedBy })
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (auditValues.Count == 0)
        {
            return;
        }

        var users = await _dbContext.Users
            .AsNoTracking()
            .Where(user =>
                auditValues.Contains(user.Id)
                || (user.UserName != null && auditValues.Contains(user.UserName))
                || (user.Email != null && auditValues.Contains(user.Email)))
            .Select(user => new
            {
                user.Id,
                user.FullName,
                user.UserName,
                user.Email
            })
            .ToListAsync(cancellationToken);

        var displayByAuditValue = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var user in users)
        {
            var displayName = GetReadableUserName(user.FullName, user.UserName, user.Email);

            AddAuditDisplayName(displayByAuditValue, user.Id, displayName);
            AddAuditDisplayName(displayByAuditValue, user.UserName, displayName);
            AddAuditDisplayName(displayByAuditValue, user.Email, displayName);
        }

        foreach (var customer in customers)
        {
            customer.CreatedBy = ResolveAuditDisplayValue(customer.CreatedBy, displayByAuditValue);
            customer.UpdatedBy = ResolveAuditDisplayValue(customer.UpdatedBy, displayByAuditValue);
        }
    }

    private static void AddAuditDisplayName(IDictionary<string, string> displayByAuditValue, string? auditValue, string displayName)
    {
        if (!string.IsNullOrWhiteSpace(auditValue) && !displayByAuditValue.ContainsKey(auditValue))
        {
            displayByAuditValue.Add(auditValue, displayName);
        }
    }

    private static string GetReadableUserName(string? fullName, string? userName, string? email)
    {
        if (!string.IsNullOrWhiteSpace(fullName))
        {
            return fullName;
        }

        if (!string.IsNullOrWhiteSpace(userName))
        {
            return userName;
        }

        return email ?? string.Empty;
    }

    private static string? ResolveAuditDisplayValue(string? auditValue, IReadOnlyDictionary<string, string> displayByAuditValue)
    {
        if (string.IsNullOrWhiteSpace(auditValue))
        {
            return null;
        }

        if (displayByAuditValue.TryGetValue(auditValue, out var displayName))
        {
            return displayName;
        }

        return Guid.TryParse(auditValue, out _) ? string.Empty : auditValue;
    }

    private string? GetCurrentUserIdentifier()
    {
        return _currentUserService.UserId ?? _currentUserService.UserName;
    }

    private sealed record CustomerMetadata(string Title, IReadOnlyList<string> Interests);
}
