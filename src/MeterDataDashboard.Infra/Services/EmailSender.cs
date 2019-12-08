using System;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace MeterDataDashboard.Infra.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            throw new NotImplementedException();
        }
    }
}
