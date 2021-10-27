using EEGTelemetryAPI.Application;
using EEGTelemetryAPI.Models;
using EEGTelemetryAPI.Models.DBContext;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace EEGTelemetryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private IWebHostEnvironment Environment;

        private readonly ILogger<AdminController> _logger;
        private readonly IAdminService _adminService;
        //private readonly IReportService _reportService;

        public AdminController(IWebHostEnvironment _environment,ILogger<AdminController> logger,IAdminService adminService)
        {
          _logger = logger;
          _adminService = adminService;
            Environment = _environment;
            //_reportService = reportService;
        }

        [HttpPost]
        public async Task<Admin> LoginAttempt(LoginData loginData)
        {    
          return await _adminService.LoginAttempt(loginData);
        }

        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        [Route("addScanReport")]
        public async Task<IActionResult> AddScanReport(IFormCollection data)
        {

            string wwwPath = this.Environment.WebRootPath;
            string contentPath = this.Environment.ContentRootPath;
            string a = data["reportDetails"];
            ReportDetails reportDetails = JsonConvert.DeserializeObject<ReportDetails>(data["reportDetails"]);
            string path = Path.Combine(this.Environment.WebRootPath, "Uploads");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            List<string> uploadedFiles = new List<string>();
            string fileName = Path.GetFileName(data.Files[0].FileName);
            using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
            {
                data.Files[0].CopyTo(stream);
                uploadedFiles.Add(fileName);
                var httpClient = new HttpClient();
                Task<string> response =  _adminService.AddScanReport(reportDetails, data.Files[0].FileName);
                return Ok(response);
            }

           
        }

        [HttpGet]
        [Route("copyFileToTemp")]
        public async Task<string> copyFileToTemp(string fileName = "")
        {
            
            string destFile = string.Empty;
            string wwwPath = this.Environment.WebRootPath;
            string contentPath = this.Environment.ContentRootPath;
            string path = Path.Combine(this.Environment.WebRootPath, "Uploads");
            string targetPath = Path.Combine(this.Environment.WebRootPath, "Temp");
            if (System.IO.Directory.Exists(path))
            {
                string file = Path.Combine(path,fileName);
                DirectoryInfo dir = new DirectoryInfo(targetPath);
                foreach (FileInfo fi in dir.GetFiles())
                {
                    fi.Delete();
                }
                fileName = System.IO.Path.GetFileName(file);
                destFile = System.IO.Path.Combine(targetPath, fileName);
                System.IO.File.Copy(file, destFile, true);
                
            }
            return "Success";

        }



            [HttpPost]
        [Route("submitReport")]
        public async Task<IActionResult> submitReport(Report report)
        {           
           Task<string> response = _adminService.submitReport(report);
           return Ok(response);
        }

        [HttpPost]
        [Route("reportSubmitted")]
        public async Task<IActionResult> isReportSubmitted(int[] ids)
        {
            Task<string> response = _adminService.isReportSubmitted(ids);
            return Ok(response);
        }

        [HttpPost]
        [Route("notificationAlertSeen")]
        public async Task<IActionResult> notificationAlertSeen(int[] ids)
        {
            Task<string> response = _adminService.notificationAlertSeen(ids);
            return Ok(response);
        }

        [HttpGet]
        [Route("getReportList")]
        public async Task<List<Report>> GetReportList()
        {
            return await _adminService.GetReportList();
        }

        [HttpGet]
        [Route("getReportData")]
        public async Task<Report> GetReportData(int reportID = 3)  
        {
            return await _adminService.GetReportData(reportID);
        }



    }
}
