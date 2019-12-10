using System;
using System.Collections.Generic;
using System.Text;

namespace MeterDataDashboard.Application.Common
{
    public interface ICurrentUserService
    {
        string UserId { get; }
    }
}
