using ExhibitionGuestManager.Application.DTOs;

namespace ExhibitionGuestManager.Application.Interfaces;

public interface IExcelExportService
{
    byte[] ExportCustomersToExcel(IReadOnlyList<CustomerDto> customers);
}
