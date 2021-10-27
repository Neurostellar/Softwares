using EEGTelemetryAPI.Application;
using EEGTelemetryAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EEGTelemetryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : Controller
    {

        private readonly ILogger<ReportController> _logger;
        private readonly IReportService _reportService;

        public ReportController(ILogger<ReportController> logger, IReportService reportService)
        {
            _logger = logger;
            _reportService = reportService;
        }

       
    }
}
