using MySql.Data.MySqlClient;
using POP_Project.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POP_Project.Repositories
{
    public class UserRepository
    {
        private readonly string mySqlConn = ConfigurationManager.ConnectionStrings["MySqlConn"].ConnectionString;

        public async Task<List<User>> GetUsersAsync()
        {
            var users = new List<User>();

            using var conn = new MySqlConnection(mySqlConn);
            await conn.OpenAsync();

            string sql = "SELECT * FROM users";

            using var cmd = new MySqlCommand(sql, conn);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                users.Add(new User
                {
                    Id = reader.GetString(reader.GetOrdinal("id")),
                    Pwd = reader.GetString(reader.GetOrdinal("pwd")),
                    Name = reader.GetString(reader.GetOrdinal("name"))
                });
            }

            return users;
        }
    }
}
