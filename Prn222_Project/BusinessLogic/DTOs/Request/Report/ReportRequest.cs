using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Enums;

namespace BusinessLogic.DTOs.Request.Report
{
    public class ReportRequest
    {
        public int SellerId { get; set; }
        public ReportTypeEnum ReportType { get; set; }
        public ReportTimeEnum ReportTime { get; set; }

        // thêm:
        // filter
        public int Month { get; set; }      // 1-12, 0 = tháng hiện tại
        public int? Week { get; set; }

    }
}
