using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.PowerBI.Api.Beta.Models;

namespace AmsPowerBiService.Models
{
    
    public class ReportWithToken : Report
    {
        public string AccessToken { get; set; }

        public ReportWithToken(Report report, string accessToken = null)
            : base(report.Id, report.Name, report.WebUrl, report.EmbedUrl)
        {
            AccessToken = accessToken;
        }
    }
}
