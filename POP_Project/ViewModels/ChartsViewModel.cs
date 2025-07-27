using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.X509;
using POP_Project.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;


namespace POP_Project.ViewModels
{
    public partial class ChartsViewModel : ObservableObject
    {

        [ObservableProperty]
        private int currentProduction;

        [ObservableProperty]
        private int defectCount;

        private DispatcherTimer timer;

        public ChartsViewModel()
        {
            StartTimer();
        }

        private void StartTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Tick += (s, e) => UpdateProductionCount();
            timer.Tick += (s, e) => UpdateDefectCount();
            timer.Tick += (s, e) => UpdateLine1GoodCount();
            timer.Tick += (s, e) => UpdateLine2GoodCount();
            timer.Tick += (s, e) => UpdateAim();
            timer.Tick += (s, e) => OnPropertyChanged(nameof(Efficiency_Line1));
            timer.Tick += (s, e) => OnPropertyChanged(nameof(Efficiency_Line2));
            timer.Tick += (s, e) => OnPropertyChanged(nameof(DefectRate));
            timer.Tick += (s, e) => OnPropertyChanged(nameof(Efficiency_Average));

            timer.Tick += (s, e) => LoadMemoAsync();


            timer.Start();
        }

        public void UpdateProductionCount() // 현재 생산량 CurrentProduction
        {
            string connStr = "server=localhost;user=root;password=1111;database=pop_project;";
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string query = "SELECT COUNT(production_time) FROM product";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                CurrentProduction = Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public void UpdateDefectCount() // 불량 수량 DefectCount
        {
            string connStr = "server=localhost;user=root;password=1111;database=pop_project;";
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM product WHERE defective_or_not = '1'";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                DefectCount = Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        //불량률 DefectCount / CurrentProduction
        public double DefectRate =>
        CurrentProduction == 0 ? 0 : Math.Round((double)DefectCount / CurrentProduction * 100, 2);

        partial void OnCurrentProductionChanged(int value)
        {
            OnPropertyChanged(nameof(DefectRate));
            //radialGauge_aim();
            //OnPropertyChanged(nameof(Aim));
        }

        partial void OnDefectCountChanged(int value)
        {
            OnPropertyChanged(nameof(DefectRate));
        }


        // -------------------------- //


        [ObservableProperty]
        private int targetProduction = 100;

        [ObservableProperty]
        private double aim;

        partial void OnTargetProductionChanged(int value)
        {
            //radialGauge_aim();
            //OnPropertyChanged(nameof(Aim));
            OnPropertyChanged(nameof(Efficiency_Line1));
        }


        private void UpdateAim()
        {
            if (CurrentProduction >= 100)
                Aim = 100;
            else
                Aim = (double)CurrentProduction;
        }

        // -------------------------- //


        [ObservableProperty]
        private int line1GoodCount;

        [ObservableProperty]
        private int line2GoodCount;

        public void UpdateLine1GoodCount() // 1번 라인 양품생산
        {
            string connStr = "server=localhost;user=root;password=1111;database=pop_project;";
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM product Where defective_or_not = 0 AND status = '반제품'";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                Line1GoodCount = Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
        public void UpdateLine2GoodCount() // 2번 라인 양품생산
        {
            string connStr = "server=localhost;user=root;password=1111;database=pop_project;";
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM product Where defective_or_not = 0 AND status = '완제품'";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                Line2GoodCount = Convert.ToInt32(cmd.ExecuteScalar());
            }
        }


        //불량률 DefectCount / CurrentProduction
        public double Efficiency_Line1 =>
        TargetProduction == 0 ? 0 : Math.Round((double)Line1GoodCount / TargetProduction * 100, 2);
        public double Efficiency_Line2 =>
        TargetProduction == 0 ? 0 : Math.Round((double)Line2GoodCount / TargetProduction * 100, 2);

        partial void OnLine1GoodCountChanged(int value)
        {
            OnPropertyChanged(nameof(Efficiency_Line1));
            OnPropertyChanged(nameof(Efficiency_Average));
        }

        partial void OnLine2GoodCountChanged(int value)
        {
            OnPropertyChanged(nameof(Efficiency_Line2));
            OnPropertyChanged(nameof(Efficiency_Average));
        }

        public double Efficiency_Average =>
        Math.Round(((double)Efficiency_Line1 + (double)Efficiency_Line2) / 2, 2);

        // -------------------------- // 제품이력

        public void Update() // 불량 수량 DefectCount
        {
            string connStr = "server=localhost;user=root;password=1111;database=pop_project;";
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM product WHERE defective_or_not = '1'";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                DefectCount = Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
        [ObservableProperty]
        private string workerName = AppLoginInfo.CurrentUserId;



        //------- 인수인계 -------//

        [ObservableProperty]
        private string handoverText;

        [ObservableProperty]
        private string allHandoverText;


        public async Task LoadMemoAsync()
        {
            string connStr = "server=localhost;user=root;password=1111;database=pop_project;";
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                await conn.OpenAsync();
                string query = "SELECT name, handover FROM users WHERE handover IS NOT NULL AND handover != ''";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    var sb = new StringBuilder();

                    while (await reader.ReadAsync())
                    {
                        string name = reader["name"].ToString();
                        string handover = reader["handover"].ToString();
                        sb.AppendLine($"{handover} - {name}");
                    }

                    AllHandoverText = sb.ToString();
                }
            }
        }

        [RelayCommand]
        public async Task SaveMemoAsync()
        {
            string connStr = $"server=127.0.0.1;user=root;password=1111;database=pop_project;";

            using var conn = new MySqlConnection(connStr);
            await conn.OpenAsync();

            string query = "UPDATE users SET handover = @memo WHERE id = @userId";

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@memo", HandoverText);
            cmd.Parameters.AddWithValue("@userId", AppLoginInfo.CurrentUserId);

            await cmd.ExecuteNonQueryAsync();
        }
    }
}


