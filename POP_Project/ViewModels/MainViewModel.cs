using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POP_Project.Models;
using POP_Project.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace POP_Project.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        public LoginViewModel LoginViewModel { get; set; } = new LoginViewModel();
        public ConditionViewModel ConditionVM { get; } = new ConditionViewModel();

        [ObservableProperty]
        private bool isMenuOpen = false;

        [ObservableProperty]
        private bool isHamburgerVisible = false;

        [ObservableProperty]
        private ProductViewModel productVM = new ProductViewModel();

        [ObservableProperty]
        private DefectViewModel defectVM = new DefectViewModel();

        [ObservableProperty]
        private ChartsViewModel chartsVM = new ChartsViewModel();

        [ObservableProperty]
        private FacilityViewModel facilityVM = new FacilityViewModel();

        [ObservableProperty]
        private PerformanceViewModel performanceVM = new PerformanceViewModel();




        [RelayCommand]
        private void OpenMenu()
        {
            // 햄버거 버튼 클릭 시 메뉴 열기 로직
            IsMenuOpen = !IsMenuOpen;
        }

        [RelayCommand]
        private void NavigatePerformance()
        {
            // PerformancePage로 이동할 때,
            // MainViewModel이 이미 가지고 있는 performanceVM 인스턴스를 전달한다.
            MainWindow.Instance.Navigate(new PerformancePage(this.performanceVM));
        }

        [RelayCommand]
        private void Logout()
        {
            var result = MessageBox.Show($"정말 로그아웃 하시겠습니까?", "로그아웃", MessageBoxButton.YesNo, MessageBoxImage.Information);
            // 로그인 페이지로 이동 및 초기화
            if (result == MessageBoxResult.Yes)
            {
                MainWindow.Instance.Navigate(new LoginPage(LoginViewModel));
                IsMenuOpen = false;
                IsHamburgerVisible = false;
            }
        }

        [RelayCommand]
        private void ViewerProcess()
        {
            Process viewerProcess = new Process();
            viewerProcess.StartInfo.FileName = "python";
            viewerProcess.StartInfo.Arguments = "OpenCV_Viewer.py";
            viewerProcess.StartInfo.WorkingDirectory = @".";
            viewerProcess.Start();
            MessageBox.Show("뷰어 실행완료");
        }
    }
}
