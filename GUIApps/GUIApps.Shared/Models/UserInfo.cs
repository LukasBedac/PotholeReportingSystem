using GUIApps.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUIApps.Shared.Models
{
    public class UserInfo
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public List<UserRole> Roles { get; set; } = new();
        public List<Report> Reports { get; set; } = new();
    }
}
