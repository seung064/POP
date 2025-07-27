using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POP_Project.Models
{
    public class Condition
    {
        // 온도, 습도, 오염도, 전력 공급 안정성
        public double Temperature { get; set; } // 온도
        public int Humidity { get; set; } // 습도
        public double Pollution { get; set; } // 오염도
        public bool PowerStability { get; set; } // 전력 공급 안정성
        public string PowerStabilityText => PowerStability ? "양호" : "불량"; // 전력 공급 안정성
        public DateTime Create_date { get; set; }
    }
}
