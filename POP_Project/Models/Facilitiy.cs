using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POP_Project.Models
{
    public partial class Facility : ObservableObject
    {
        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private int running_Time;

        [ObservableProperty]
        private DateTime recent_Check;

        [ObservableProperty]
        private string manufacturer;

        [ObservableProperty]
        private string model_Name;
    }
}
