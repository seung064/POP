﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POP_Project.Models
{
    public class Product
    {
        public int QR_Code { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public DateTime Production_Time { get; set; }
        public bool Defective_or_not { get; set; }
    }
}
