using System.Configuration;
using System.Data;
using System.Windows;
using System.Diagnostics;

namespace POP_Project
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            this.DispatcherUnhandledException += (s, e) =>
            {
                MessageBox.Show(e.Exception.Message);
                e.Handled = true;
            };

        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            StartDectorProcess(); // 프로그램 시작 시 자동 실행

            var mainWindow = new MainWindow();
            mainWindow.Show();
        }

        Process detectorProcess = null;
        public void StartDectorProcess()
        {
            detectorProcess = new Process();
            detectorProcess.StartInfo.FileName = "python";
            detectorProcess.StartInfo.Arguments = "OpenCV_Detector.py";
            detectorProcess.StartInfo.WorkingDirectory = @".";
            detectorProcess.Start();
            MessageBox.Show("실행완료");
        }
    }
}
