﻿using System.ComponentModel.DataAnnotations;

namespace MeterDataDashboard.Web.Models.UserMgmt
{
    public class UserEditVM
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string UserRole { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public bool IsTwoFactorEnabled { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }
    }
}
