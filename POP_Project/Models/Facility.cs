using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POP_Project.Models
{
    public class Facility
    {
        public string Name { get; set; }
        public int Running_Time { get; set; }
        public DateTime Recent_Check { get; set; }
        public string Manufacturer { get; set; }
        public string Model_Name { get; set; }
    }
}
