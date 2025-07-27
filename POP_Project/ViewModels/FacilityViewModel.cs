using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POP_Project.Models;
using POP_Project.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POP_Project.ViewModels
{
    public partial class FacilityViewModel : ObservableObject
    {
        private FacilityRepository facilityRepo = new FacilityRepository();

        [ObservableProperty] private Facility pcbLoader;
        [ObservableProperty] private Facility screenPrinter;
        [ObservableProperty] private Facility chipMounter;
        [ObservableProperty] private Facility reflowOven;

        public async Task LoadFacilitiesAsync()
        {
            PcbLoader = await facilityRepo.GetFacilityByNameAsync("PCB Loader");
            ScreenPrinter = await facilityRepo.GetFacilityByNameAsync("Screen Printer");
            ChipMounter = await facilityRepo.GetFacilityByNameAsync("Chip Mounter");
            ReflowOven = await facilityRepo.GetFacilityByNameAsync("Reflow Oven");
        }
    }
}
