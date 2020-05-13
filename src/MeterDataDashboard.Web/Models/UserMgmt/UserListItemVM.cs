﻿using System;
using System.Linq;
using System.Threading.Tasks;

namespace MeterDataDashboard.Web.Models.UserMgmt
{
    public class UserListItemVM
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string UserRole { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public string Phone { get; set; }
    }
}
