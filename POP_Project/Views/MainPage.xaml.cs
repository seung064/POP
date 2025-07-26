using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using POP_Project.ViewModels;

namespace POP_Project.Views
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        private FacilityViewModel FacilityVM;
        
        public MainPage()
        {
            InitializeComponent();

            FacilityVM = new FacilityViewModel();
            this.DataContext = FacilityVM;

            // 비동기 데이터 로딩
            Loaded += async (_, __) => await FacilityVM.LoadFacilitiesAsync();
        }

        public MainPage(FacilityViewModel mvm)
        {
            InitializeComponent();
            DataContext = mvm;
        }
    }
}
