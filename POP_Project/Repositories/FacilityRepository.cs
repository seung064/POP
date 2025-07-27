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
    public class FacilityRepository
    {
        private readonly string mySqlConn = ConfigurationManager.ConnectionStrings["MySqlConn"].ConnectionString;

        public async Task<Facility> GetFacilityByNameAsync(string name)
        {
            Facility facility = null;

            using var conn = new MySqlConnection(mySqlConn);
            await conn.OpenAsync();

            string sql = "SELECT * FROM facility WHERE name = @name LIMIT 1";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@name", name);

            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                facility = new Facility
                {
                    Name = reader["name"].ToString(),
                    Running_Time = Convert.ToInt32(reader["running_time"]),
                    Recent_Check = Convert.ToDateTime(reader["recent_check"]),
                    Manufacturer = reader["manufacturer"].ToString(),
                    Model_Name = reader["Model_name"].ToString()
                };
            }
            return facility;
        }
    }
}
