using MySql.Data.MySqlClient;
using MySql.Data.Types;
using POP_Project.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
using static Google.Protobuf.Reflection.SourceCodeInfo.Types;

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
                    Time_defect = reader.IsDBNull(reader.GetOrdinal("time_defect")) ? DateTime.MinValue : reader.GetDateTime("time_defect"),
                    QR_Code = reader.GetInt32(reader.GetOrdinal("qr_code")),
                    Status = reader.GetString(reader.GetOrdinal("status")),
                    Name = reader.GetString(reader.GetOrdinal("name")),
                    Class_defect = reader.IsDBNull(reader.GetOrdinal("class_defect"))
                       ? "NULL"
                       : reader.GetString(reader.GetOrdinal("class_defect")),

                    Location = reader.IsDBNull(reader.GetOrdinal("location"))
                       ? 0
                       : reader.GetInt32(reader.GetOrdinal("location"))
                });
            }

            return defects;
        }
    }
}