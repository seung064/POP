using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Converters;

namespace POP_Project.Models
{
    public class UserAndEmployeeInfo
    {
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }

        public DateTime HireDate { get; set; }
        public string HireDateFormatted => HireDate.ToString("yyyy/MM/dd");
        public string EmployeeStatus { get; set; }
        public bool SafetyEducation { get; set; }

        public string Pwd { get; set; }

        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public bool IsDelete { get; set; }
        //public string IsDeleteFormatted => IsDelete ? "Y" : "N";
    }
}
