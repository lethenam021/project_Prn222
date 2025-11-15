using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.DTOs.Request.Report;
using BusinessLogic.DTOs.Response.Report;

namespace BusinessLogic.Interface
{
    public interface IReportService
    {
        Task<IEnumerable<ReportResponse>> GetReportAsync(ReportRequest reportRequest);
    }
}
