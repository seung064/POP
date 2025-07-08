using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using POP_Project.Models;
using POP_Project.Repositories;
using POP_Project.Views;

namespace POP_Project.ViewModels
{
    public partial class VisionViewModel : ObservableObject
    {

        [RelayCommand]
        private void NavigateBack()
        {
            // MainPage로 이동
            MainWindow.Instance.Navigate(new MainPage());
        }
    }
}
