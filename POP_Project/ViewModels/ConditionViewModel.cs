using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveCharts;
using LiveCharts.Wpf;
using POP_Project.Models;
using POP_Project.Repositories;
using POP_Project.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;

namespace POP_Project.ViewModels
{
    public partial class ConditionViewModel : ObservableObject
    {
        private readonly ConditionRepository conditionRepo = new ConditionRepository();

        // LiveCharts의 시리즈 컬렉션 (줄 3개: 온도, 습도, 오염도)
        [ObservableProperty]
        private SeriesCollection seriesCollection;

        //  x축 레이블 (시간)
        [ObservableProperty]
        private ObservableCollection<string> xLabels = new();

        // 차트 갱신용 타이머
        private DispatcherTimer timer;

        private Random random = new();

        // 그래프 값 포맷 함수 (Y축)
        public Func<double, string> ValueFormatter { get; set; }

        public ConditionViewModel()
        {
            InitChart();        // 차트 초기화
            StartTimer();       // DB 저장 타이머 시작
            StartChartTimer();  // 차트 업데이트 타이머 시작
        }

        // 차트 업데이트 타이머
        private void StartChartTimer()
        {
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1) // 1초마다 차트 업데이트
            };
            // 타이머 틱마다 UpdateChart 메서드 호출
            timer.Tick += (s, e) => UpdateChart(); // 랜덤값 기반
            timer.Start();
        }

        private DispatcherTimer saveTimer;

        // DB 저장 타이머
        private void StartTimer()
        {
            saveTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1) // 1초마다 DB에 저장
            };
            saveTimer.Tick += async (s, e) =>
            {
                var newData = new POP_Project.Models.Condition
                {
                    Temperature = 25 + random.NextDouble() * 5, // 온도: 25~30도 사이
                    Humidity = random.Next(40, 70),             // 습도: 40~70% 사이
                    Pollution = random.NextDouble() * 10,       // 오염도: 0~10 사이
                    PowerStability = true                       // 전력 공급 안정성: true 고정
                };

                await conditionRepo.InsertConditionAsync(newData);
            };
            saveTimer.Start();
        }

        private void InitChart()
        {
            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "온도",                               // 온도 시리즈
                    Values = new ChartValues<double>(),           // 데이터 값
                    PointGeometry = DefaultGeometries.Circle,     // 원형
                    StrokeThickness = 1                           // 선 두께
                },
                new LineSeries
                {
                    Title = "습도",
                    Values = new ChartValues<double>(),
                    PointGeometry = DefaultGeometries.Circle,
                    StrokeThickness = 1
                },
                new LineSeries
                {
                    Title = "오염도",
                    Values = new ChartValues<double>(),
                    PointGeometry = DefaultGeometries.Circle,
                    StrokeThickness = 1
                }
            };

            // x축 레이블 초기화
            XLabels = new ObservableCollection<string> { };

            // Y축 값 포맷 함수 정의(소수점 첫째자리까지 표시)
            ValueFormatter = val => $"{val:F1}";
        }

        // 차트 업데이트 메서드
        private async void UpdateChart()
        {
            // 랜덤 측정값 생성
            double temperature = Math.Round(20 + 5 * random.NextDouble(), 1); // 온도: 20~25 랜덤
            double humidity = Math.Round(40 + 20 * random.NextDouble(), 1);   // 습도: 40~60 랜덤
            double pollution = Math.Round(100 + 300 * random.NextDouble(), 1);// 오염도: 100~400 랜덤

            // 시각 레이블 추가
            string time = DateTime.Now.ToString("HH:mm:ss");
            xLabels.Add(time);
            if (xLabels.Count > 10) xLabels.RemoveAt(0);

            // 그래프 데이터 추가 (최대 10개 유지)
            Application.Current.Dispatcher.Invoke(() =>
            {
                var temp = SeriesCollection[0].Values;
                var humi = SeriesCollection[1].Values;
                var poll = SeriesCollection[2].Values;

                temp.Add(temperature);
                if (temp.Count > 10) temp.RemoveAt(0);

                humi.Add(humidity);
                if (humi.Count > 10) humi.RemoveAt(0);

                poll.Add(pollution);
                if (poll.Count > 10) poll.RemoveAt(0);
            });
            //try
            //{
            //    // 최신 30개 데이터 조회
            //    var latestData = await conditionRepo.GetLatestConditionsAsync(30);

            //    // 시리즈 값 초기화
            //    var tempValues = new ChartValues<double>();
            //    var humiValues = new ChartValues<double>();
            //    var pollValues = new ChartValues<double>();
            //    var labels = new ObservableCollection<string>();

            //    foreach (var cd in latestData)
            //    {
            //        tempValues.Add(cd.Temperature);
            //        humiValues.Add(cd.Humidity);
            //        pollValues.Add(cd.Pollution);
            //        labels.Add(cd.Create_date.ToString("HH:mm:ss"));
            //    }

            //    Application.Current.Dispatcher.Invoke(() =>
            //    {
            //        // 차트 시리즈 갱신
            //        SeriesCollection[0].Values = tempValues;
            //        SeriesCollection[1].Values = humiValues;
            //        SeriesCollection[2].Values = pollValues;

            //        // X축 레이블 갱신
            //        XLabels.Clear();
            //        foreach (var label in labels)
            //            XLabels.Add(label);
            //    });
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("차트 업데이트 중 오류 발생: " + ex.Message);
            //}
        }
    }
}
