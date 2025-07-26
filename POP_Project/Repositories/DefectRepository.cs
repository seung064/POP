using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POP_Project.Models;
using MySql.Data.MySqlClient;
using MySql.Data.Types;
using System.Data;
using System.Threading.Tasks;
using System.Configuration;

namespace POP_Project.Repositories
{
    public class DefectRepository
    {
        private readonly string mySqlConn = ConfigurationManager.ConnectionStrings["MySqlConn"].ConnectionString;

        public async Task<List<Defect>> GetdefectAsync()
        {
            var defects = new List<Defect>();

            using var conn = new MySqlConnection(mySqlConn);
            await conn.OpenAsync();

            string sql = "SELECT * FROM defect";

            using var cmd = new MySqlCommand(sql, conn);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                defects.Add(new Defect
                {
                    TaskId = reader.GetInt32("taskId"),
                    QR_Code = reader.GetString(reader.GetOrdinal("qr_code")),
                    Status = reader.GetString(reader.GetOrdinal("status")),
                    Production_Time = reader.GetDateTime("production_time"),
                    Location = reader.GetInt32("location")
                });
            }

            return defects;
        }
    }
}