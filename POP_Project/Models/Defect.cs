using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POP_Project.Models
{
    public class Defect
    {
        public int TaskId { get; set; }
        public string QR_Code { get; set; }
        public string Status { get; set; }
        public DateTime Production_Time { get; set; }
        public int Location { get; set; }
    }
}
