using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POP_Project.Models;
using POP_Project.Repositories;
using POP_Project.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace POP_Project.ViewModels
{
    public partial class VisionViewModel : ObservableObject
    {

        [RelayCommand]
        private void NavigateBack()
        {
            // MainPage로 이동
            MainWindow.Instance.Navigate(new MainPage(MainWindow.Instance.MainVM));
        }


        [RelayCommand]
        private void ViewerProcess()
        {
            Process viewerProcess = new Process();
            viewerProcess.StartInfo.FileName = "python";
            viewerProcess.StartInfo.Arguments = "OpenCV_Viewer.py";
            viewerProcess.StartInfo.WorkingDirectory = @"C:\Users\o\Desktop\xaml\POP_Project\bin\Debug\net8.0-windows";
            viewerProcess.Start();
            MessageBox.Show("뷰어 실행완료");
        }
    }
}
