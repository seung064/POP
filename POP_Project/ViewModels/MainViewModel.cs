using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POP_Project.Models;
using POP_Project.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace POP_Project.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        public LoginViewModel LoginViewModel { get; set; } = new LoginViewModel();

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
        private PerformanceViewModel performanceVM = new PerformanceViewModel();

        [ObservableProperty]
        private FacilityViewModel facilityVM = new FacilityViewModel();


        [RelayCommand]
        private void OpenMenu()
        {
            // 햄버거 버튼 클릭 시 메뉴 열기 로직
            IsMenuOpen = !IsMenuOpen;
        }

        
        [RelayCommand]
        private void NavigateVision()
        {
            // VisionPage로 이동
            MainWindow.Instance.Navigate(new VisionPage());
        }


        [RelayCommand]
        private void NavigatePerformance()
        {
            // PerformancePage로 이동할 때,
            // MainViewModel이 이미 가지고 있는 performanceVM 인스턴스를 전달한다.
            MainWindow.Instance.Navigate(new PerformancePage(this.PerformanceVM));
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
    }
}
