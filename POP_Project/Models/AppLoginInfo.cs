using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POP_Project.Models
{
    public class AppLoginInfo
    {
        public static string CurrentUserId { get; set; }
        public static string CurrentUserPwd { get; set; }
        public static string CurrentUserName { get; set; }
    }
}