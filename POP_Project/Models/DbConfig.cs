using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POP_Project.Models
{
    public class DbConfig // 로그인 DB 정보 저장 모델 - 인수인계용
    {
        public string UserId { get; set; }
        public string Password { get; set; }
        public static string UserName { get; set; }
    }

}
