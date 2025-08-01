using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POP_Project.Models
{
    public class User
    {
        public int TaskId { get; set; }
        public string Id { get; set; }
        public string Pwd { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return Id;
        }
    }     
}
