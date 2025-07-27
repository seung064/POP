using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POP_Project.Models;
using POP_Project.Repositories;
using POP_Project.Views;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace POP_Project.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private UserRepository userRepo = new UserRepository();

        [ObservableProperty]
        private string id;

        [ObservableProperty]
        private string name;
        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private ObservableCollection<User> user = new ObservableCollection<User>();

        public LoginViewModel()
        {
            UsersInfo();
        }

        private async void UsersInfo()
        {
            List<User> userList = await userRepo.GetUsersAsync();
            User = new ObservableCollection<User>(userList);
        }

        //public DbConfig DbConfig { get; private set; } = new DbConfig(); // 인수인계용

        [RelayCommand]
        private async Task Login()
        {
            bool check = false;

            foreach (var u in User)
            {
                if (u.Id.Equals(Id) && u.Pwd.Equals(Password))
                {
                    check = true;
                    Name = u.Name;

                    // DB 계정 정보 저장 - 인수인계용
                    AppLoginInfo.CurrentUserId = u.Id;
                    AppLoginInfo.CurrentUserPwd = u.Pwd;
                    AppLoginInfo.CurrentUserName = u.Name;

                    MessageBox.Show($"{u.Name}님 환영합니다", "로그인", MessageBoxButton.OK, MessageBoxImage.Information);
                    MainWindow.Instance.Navigate(new MainPage());
                    MainWindow.Instance.MainVM.IsMenuOpen = false;

                    MainWindow.Instance.MainVM.IsHamburgerVisible = true;
                    break;
                }
            }

            if (!check)
            {
                MessageBox.Show("로그인 실패", "로그인", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
