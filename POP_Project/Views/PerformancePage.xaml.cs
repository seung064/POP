using POP_Project.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
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
using POP_Project.Converters;

namespace POP_Project.Views
{
    /// <summary>
    /// Interaction logic for PerformancePage.xaml
    /// </summary>
    public partial class PerformancePage : Page
    {
        public PerformancePage()
        {
            InitializeComponent();
            this.DataContext = new PerformanceViewModel();
        }
    }
}
