using ClosedXML.Excel;
using ExhibitionGuestManager.Application.DTOs;
using ExhibitionGuestManager.Application.Interfaces;

namespace ExhibitionGuestManager.Infrastructure.Services;

public class ExcelExportService : IExcelExportService
{
    private static readonly IReadOnlyDictionary<string, string> TitleLabels = new Dictionary<string, string>
    {
        ["MrMrs"] = "Mr./Ms.",
        ["Teacher"] = "Teacher",
        ["Doctor"] = "Doctor",
        ["Other"] = "Other"
    };

    private static readonly IReadOnlyDictionary<string, string> InterestLabels = new Dictionary<string, string>
    {
        ["CurriculumBooks"] = "Curriculum Books",
        ["TrainingKits"] = "Training Kits",
        ["RoboticsLab"] = "Robotics Lab",
        ["ProfessionalExamPreparationCourses"] = "Professional Exam Preparation Courses",
        ["EnglishExamPreparationCourses"] = "English Exam Preparation Courses",
        ["BilingersBilingualKindergartenProgram"] = "Bilingers Bilingual Kindergarten Program",
        ["TeacherTrainingWorkshops"] = "Teacher Training Workshops",
        ["AfterSchoolMentalArithmeticProgram"] = "After-School Mental Arithmetic Program",
        ["AfterSchoolProgramsPackage"] = "After-School Programs Package",
        ["ArticRobots"] = "Artic Robots",
        ["ACEBOTTRobots"] = "ACEBOTT Robots",
        ["HelloCodeProgrammingProgram"] = "HelloCode Programming Program",
        ["OtherInterest"] = "Other"
    };

    public byte[] ExportCustomersToExcel(IReadOnlyList<CustomerDto> customers)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Customers");

        var headers = new[]
        {
            "Title",
            "Name",
            "Position",
            "Mobile Number",
            "City",
            "Organization Name",
            "Email",
            "Interests",
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
        worksheet.Column(4).Style.NumberFormat.Format = "@";

        for (var index = 0; index < customers.Count; index++)
        {
            var customer = customers[index];
            var row = index + 2;

            worksheet.Cell(row, 1).Value = FormatTitle(customer.Title);
            worksheet.Cell(row, 2).Value = customer.Name;
            worksheet.Cell(row, 3).Value = customer.Position;
            worksheet.Cell(row, 4).Value = customer.MobileNumber;
            worksheet.Cell(row, 4).Style.NumberFormat.Format = "@";
            worksheet.Cell(row, 5).Value = customer.City;
            worksheet.Cell(row, 6).Value = customer.OrganizationName;
            worksheet.Cell(row, 7).Value = customer.Email;
            worksheet.Cell(row, 8).Value = FormatInterests(customer.Interests);
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

    private static string FormatInterests(IEnumerable<string> interests)
    {
        return string.Join(", ", interests.Select(interest =>
            InterestLabels.TryGetValue(interest, out var label) ? label : interest));
    }

    private static string FormatTitle(string title)
    {
        return TitleLabels.TryGetValue(title, out var label) ? label : title;
    }
}
