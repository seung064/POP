using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;
using POP_Project.Models;
using POP_Project.Repositories;
using POP_Project.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POP_Project.ViewModels
{
    public partial class DefectViewModel : ObservableObject
    {
        private DefectRepository defectRepo = new DefectRepository();

        [ObservableProperty]
        private ObservableCollection<Defect> defects = new ObservableCollection<Defect>();

        [ObservableProperty]
        private int defectInfo;

        public DefectViewModel()
        {
            AllDefectInfo();
        }

        private async Task AllDefectInfo()
        {
            var defectList = await defectRepo.GetdefectAsync();

            Defects.Clear();
            foreach (var defect in defectList)
            {
                Defects.Add(defect);
            }
        }
    }
}
