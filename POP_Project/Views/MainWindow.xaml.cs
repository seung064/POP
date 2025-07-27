using POP_Project.Models;
using POP_Project.ViewModels;
using POP_Project.Views;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace POP_Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance { get; private set; }

        public LoginViewModel LoginVM { get; set; }
        public MainViewModel MainVM { get; set; }
        public ProductViewModel ProductVM { get; set; }
        public DefectViewModel DefectVM { get; set; }
        public ChartsViewModel ChartsVM { get; set; }

        public FacilityViewModel FacilityVM { get; set; }
        public MainWindow()
        {
            InitializeComponent();

            Instance = this;

            LoginVM = new LoginViewModel();
            MainVM = new MainViewModel();
            this.DataContext = MainVM;

            MainFrame.Navigated += MainFrame_Navigated;

            MainFrame.Navigate(new LoginPage(LoginVM));
            //MainFrame.Navigate(new MainPage(MainVM));  //DB연동 오류로 인해 로그인 스킵
            MainVM.IsHamburgerVisible = true;
        }

        // LoginPage에서 햄버거 버튼 숨기기 (투명도 조정)
        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            // LoginPage일 경우 햄버거 버튼 숨기기
            if (e.Content is Views.LoginPage)
            {
                MainVM.IsHamburgerVisible = false;
            }
            else
            {
                MainVM.IsHamburgerVisible = true;
            }
        }

        public void Navigate(Page page)
        {
            MainFrame.Navigate(page);
        }
    }
}