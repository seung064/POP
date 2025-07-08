using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POP_Project.Models;
using POP_Project.Repositories;
using POP_Project.Views;

namespace POP_Project.ViewModels
{
    public partial class PerformanceViewModel : ObservableObject
    {
        [RelayCommand]
        private void NavigateBack()
        {
            // MainPage로 이동
            MainWindow.Instance.Navigate(new MainPage());
        }
    }
}
