using ClosedXML.Excel;
using ExhibitionGuestManager.Application.DTOs;
using ExhibitionGuestManager.Application.Interfaces;

namespace ExhibitionGuestManager.Infrastructure.Services;

public class ExcelExportService : IExcelExportService
{
    public byte[] ExportCustomersToExcel(IReadOnlyList<CustomerDto> customers)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Customers");

        var headers = new[]
        {
            "Full Name",
            "Mobile Number",
            "City",
            "Company",
            "Department",
            "Status",
            "General Notes",
            "Internal Notes",
            "Created At",
            "Created By",
            "Updated At",
            "Updated By"
        };

        for (var column = 0; column < headers.Length; column++)
        {
            worksheet.Cell(1, column + 1).Value = headers[column];
        }

        worksheet.Row(1).Style.Font.Bold = true;

        for (var index = 0; index < customers.Count; index++)
        {
            var customer = customers[index];
            var row = index + 2;

            worksheet.Cell(row, 1).Value = customer.FullName;
            worksheet.Cell(row, 2).Value = customer.MobileNumber;
            worksheet.Cell(row, 3).Value = customer.City;
            worksheet.Cell(row, 4).Value = customer.CompanyName;
            worksheet.Cell(row, 5).Value = customer.Department;
            worksheet.Cell(row, 6).Value = customer.Status.ToString();
            worksheet.Cell(row, 7).Value = customer.GeneralNotes;
            worksheet.Cell(row, 8).Value = customer.InternalNotes;
            worksheet.Cell(row, 9).Value = customer.CreatedAt;
            worksheet.Cell(row, 10).Value = customer.CreatedBy;
            worksheet.Cell(row, 11).Value = customer.UpdatedAt;
            worksheet.Cell(row, 12).Value = customer.UpdatedBy;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        return stream.ToArray();
    }
}
