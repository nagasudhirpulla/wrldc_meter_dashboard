using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MeterDataDashboard.Web.Models
{
    public class AgcUploadVm
    {
        [Required]
        public List<IFormFile> DataFiles { get; set; }
    }
}