using ZiyoNur.Service.Common;
using ZiyoNur.Service.DTOs.Reports;

namespace ZiyoNur.Service.Features.Reports.Queries;

public class GetSalesReportQuery : BaseRequest<BaseResponse<SalesReportDto>>
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string ReportType { get; set; } = "daily"; // daily, weekly, monthly

    public GetSalesReportQuery(DateTime startDate, DateTime endDate, string reportType = "daily")
    {
        StartDate = startDate;
        EndDate = endDate;
        ReportType = reportType;
    }
}