using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MySql.Data.MySqlClient;
using POP_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -------------// 인수인계 - handover
//ALTER TABLE Users
//ADD COLUMN MemoText TEXT NULL;

namespace POP_Project.ViewModels
{
    public partial class HandoverViewModel : ObservableObject
    {
        private readonly DbConfig _dbConfig;

        public HandoverViewModel(DbConfig dbConfig)
        {
            _dbConfig = dbConfig;
        }

        [ObservableProperty]
        private string handoverText;

        [RelayCommand]
        public async Task LoadMemoAsync()
        {
            string connStr = $"server=127.0.0.1;user={_dbConfig.UserId};password={_dbConfig.Password};database=pop_project;";

            using var conn = new MySqlConnection(connStr);
            await conn.OpenAsync();

            string query = "SELECT handover FROM Users WHERE UserId = @userId";

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@userId", _dbConfig.UserId);

            var result = await cmd.ExecuteScalarAsync();
            if (result != null && result != DBNull.Value)
            {
                handoverText = result.ToString();
            }
            else
            {
                handoverText = string.Empty;
            }
        }


        [RelayCommand]
        public async Task SaveMemoAsync()
        {
            string connStr = $"server=127.0.0.1;user={_dbConfig.UserId};password={_dbConfig.Password};database=pop_project;";

            using var conn = new MySqlConnection(connStr);
            await conn.OpenAsync();

            string query = "UPDATE users SET handover = @memo WHERE UserId = @userId";

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@memo", handoverText);
            cmd.Parameters.AddWithValue("@userId", _dbConfig.UserId);

            await cmd.ExecuteNonQueryAsync();
        }
    }
}