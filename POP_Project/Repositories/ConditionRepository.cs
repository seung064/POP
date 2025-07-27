using MySql.Data.MySqlClient;
using POP_Project.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace POP_Project.Repositories
{
    public class ConditionRepository
    {
        private readonly string mySqlConn = ConfigurationManager.ConnectionStrings["MySqlConn"].ConnectionString;

        // 1. 1초마다 센서값을 저장하는 메서드
        public async Task InsertConditionAsync(POP_Project.Models.Condition cd)
        {
            using var conn = new MySqlConnection(mySqlConn);
            await conn.OpenAsync();

            string sql = @"
                INSERT INTO `condition`(temperature, humidity, pollution, powerStability, create_date) 
                VALUES (@temperature, @humidity, @pollution, @powerStability, CURRENT_TIMESTAMP)";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@temperature", cd.Temperature);
            cmd.Parameters.AddWithValue("@humidity", cd.Humidity);
            cmd.Parameters.AddWithValue("@pollution", cd.Pollution);
            cmd.Parameters.AddWithValue("@powerStability", cd.PowerStability);

            await cmd.ExecuteNonQueryAsync();
        }

        // 2. 가장 최근 데이터 30개 가져오기 (실시간 차트용)
        public async Task<List<POP_Project.Models.Condition>> GetLatestConditionsAsync(int count = 30)
        {
            var conditions = new List<POP_Project.Models.Condition>();

            using var conn = new MySqlConnection(mySqlConn);
            await conn.OpenAsync();

            string sql = $@"
                SELECT temperature, humidity, pollution, powerStability, create_date
                FROM `condition`
                ORDER BY create_date DESC 
                LIMIT @count";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@count", count);

            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                conditions.Add(new POP_Project.Models.Condition
                {
                    Temperature = reader.GetDouble("temperature"),
                    Humidity = reader.GetInt32("humidity"),
                    Pollution = reader.GetDouble("pollution"),
                    PowerStability = reader.GetBoolean("powerStability"),
                    Create_date = reader.GetDateTime("create_date")
                });
            }

            conditions.Reverse(); // 시간순 정렬 (오름차순)로 변경
            return conditions;
        }
    }
}
