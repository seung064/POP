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
    public class ProductRepository
    {
        private readonly string mySqlConn = ConfigurationManager.ConnectionStrings["MySqlConn"].ConnectionString;



        public async Task<List<Product>> GetproductAsync()
        {
            var products = new List<Product>();

            using var conn = new MySqlConnection(mySqlConn);
            await conn.OpenAsync();

            string sql = "SELECT * FROM product";

            using var cmd = new MySqlCommand(sql, conn);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                products.Add(new Product
                {
                    QR_Code = reader.GetInt32("qr_code"),
                    Name = reader.GetString(reader.GetOrdinal("name")),
                    Status = reader.GetString(reader.GetOrdinal("status")),
                    Production_Time = reader.IsDBNull(reader.GetOrdinal("production_time")) ? DateTime.MinValue : reader.GetDateTime("production_time"),
                    Defective_or_not = reader.IsDBNull(reader.GetOrdinal("defective_or_not")) ? false : reader.GetBoolean("defective_or_not")
                });
            }

            return products;
        }
    }
}
