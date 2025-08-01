using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POP_Project.Models
{
    public class Defect
    {
        public int QR_Code { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public DateTime Time_defect { get; set; }
        public int Location { get; set; }
        public string Class_defect { get; set; }
    }
}
