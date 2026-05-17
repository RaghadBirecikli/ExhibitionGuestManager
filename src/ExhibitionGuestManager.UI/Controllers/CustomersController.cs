using ExhibitionGuestManager.Application.DTOs;
using ExhibitionGuestManager.Application.Interfaces;
using ExhibitionGuestManager.Application.Models;
using ExhibitionGuestManager.UI.Resources;
using ExhibitionGuestManager.UI.ViewModels.Customers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ExhibitionGuestManager.UI.Controllers;

[Authorize]
public class CustomersController : Controller
{
    private static readonly int[] AllowedPageSizes = [10, 25, 50, 100];

    private readonly ICustomerService _customerService;
    private readonly IExcelExportService _excelExportService;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public CustomersController(
        ICustomerService customerService,
        IExcelExportService excelExportService,
        IStringLocalizer<SharedResource> localizer)
    {
        _customerService = customerService;
        _excelExportService = excelExportService;
        _localizer = localizer;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        [FromQuery] CustomerFilterViewModel filter,
        CancellationToken cancellationToken)
    {
        NormalizeFilter(filter);

        var result = await _customerService.GetPagedAsync(MapFilter(filter), cancellationToken);

        var viewModel = new CustomerIndexViewModel
        {
            Customers = result.Items.Select(MapToListItemViewModel).ToList(),
            Filter = filter,
            TotalCount = result.TotalCount,
            TotalPages = result.TotalPages,
            HasPreviousPage = result.HasPreviousPage,
            HasNextPage = result.HasNextPage
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Export(
        [FromQuery] CustomerFilterViewModel filter,
        CancellationToken cancellationToken)
    {
        NormalizeFilter(filter);
        filter.PageNumber = 1;

        var customers = await _customerService.GetForExportAsync(MapFilter(filter), cancellationToken);
        var fileContent = _excelExportService.ExportCustomersToExcel(customers);
        var fileName = $"Customers_{DateTime.UtcNow:yyyyMMdd}.xlsx";

        return File(
            fileContent,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var customer = await _customerService.GetByIdAsync(id, cancellationToken);

        if (customer is null)
        {
            return NotFound();
        }

        return View(MapToDetailsViewModel(customer));
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CustomerCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CustomerCreateViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await _customerService.CreateAsync(MapToCreateDto(model), cancellationToken);
        TempData["Success"] = _localizer["CustomerCreatedSuccessfully"].Value;

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var customer = await _customerService.GetByIdAsync(id, cancellationToken);

        if (customer is null)
        {
            return NotFound();
        }

        return View(MapToEditViewModel(customer));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(CustomerEditViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var updated = await _customerService.UpdateAsync(MapToUpdateDto(model), cancellationToken);

        if (!updated)
        {
            return NotFound();
        }

        TempData["Success"] = _localizer["CustomerUpdatedSuccessfully"].Value;

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var customer = await _customerService.GetByIdAsync(id, cancellationToken);

        if (customer is null)
        {
            return NotFound();
        }

        return View(MapToDetailsViewModel(customer));
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        var deleted = await _customerService.SoftDeleteAsync(id, cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }

        TempData["Success"] = _localizer["CustomerDeletedSuccessfully"].Value;

        return RedirectToAction(nameof(Index));
    }

    private static CustomerListItemViewModel MapToListItemViewModel(CustomerDto dto)
    {
        return new CustomerListItemViewModel
        {
            Id = dto.Id,
            FullName = dto.FullName,
            MobileNumber = dto.MobileNumber,
            City = dto.City,
            CompanyName = dto.CompanyName,
            Department = dto.Department,
            Status = dto.Status,
            CreatedAt = dto.CreatedAt
        };
    }

    private static CustomerDetailsViewModel MapToDetailsViewModel(CustomerDto dto)
    {
        return new CustomerDetailsViewModel
        {
            Id = dto.Id,
            FullName = dto.FullName,
            MobileNumber = dto.MobileNumber,
            City = dto.City,
            CompanyName = dto.CompanyName,
            Department = dto.Department,
            Status = dto.Status,
            GeneralNotes = dto.GeneralNotes,
            InternalNotes = dto.InternalNotes,
            CreatedAt = dto.CreatedAt,
            CreatedBy = dto.CreatedBy,
            UpdatedAt = dto.UpdatedAt,
            UpdatedBy = dto.UpdatedBy
        };
    }

    private static CustomerEditViewModel MapToEditViewModel(CustomerDto dto)
    {
        return new CustomerEditViewModel
        {
            Id = dto.Id,
            FullName = dto.FullName,
            MobileNumber = dto.MobileNumber,
            City = dto.City,
            CompanyName = dto.CompanyName,
            Department = dto.Department,
            Status = dto.Status,
            GeneralNotes = dto.GeneralNotes,
            InternalNotes = dto.InternalNotes
        };
    }

    private static CreateCustomerDto MapToCreateDto(CustomerCreateViewModel model)
    {
        return new CreateCustomerDto
        {
            FullName = model.FullName,
            MobileNumber = model.MobileNumber,
            City = model.City,
            CompanyName = model.CompanyName,
            Department = model.Department,
            Status = model.Status,
            GeneralNotes = model.GeneralNotes,
            InternalNotes = model.InternalNotes
        };
    }

    private static UpdateCustomerDto MapToUpdateDto(CustomerEditViewModel model)
    {
        return new UpdateCustomerDto
        {
            Id = model.Id,
            FullName = model.FullName,
            MobileNumber = model.MobileNumber,
            City = model.City,
            CompanyName = model.CompanyName,
            Department = model.Department,
            Status = model.Status,
            GeneralNotes = model.GeneralNotes,
            InternalNotes = model.InternalNotes
        };
    }

    private static CustomerFilterModel MapFilter(CustomerFilterViewModel filter)
    {
        return new CustomerFilterModel
        {
            SearchTerm = filter.SearchTerm,
            City = filter.City,
            CompanyName = filter.CompanyName,
            Department = filter.Department,
            Status = filter.Status,
            FromDate = filter.FromDate,
            ToDate = filter.ToDate,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }

    private static void NormalizeFilter(CustomerFilterViewModel filter)
    {
        filter.PageNumber = Math.Max(1, filter.PageNumber);

        if (!AllowedPageSizes.Contains(filter.PageSize))
        {
            filter.PageSize = 10;
        }
    }
}
