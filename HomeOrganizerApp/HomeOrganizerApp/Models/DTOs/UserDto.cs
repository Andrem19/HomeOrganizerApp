using System;
using System.Collections.Generic;
using System.Text;

namespace HomeOrganizerApp.Models.DTOs
{
    public class UserDto
    {
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string Token { get; set; }
        public string AvatarUrl { get; set; }
    }
}
