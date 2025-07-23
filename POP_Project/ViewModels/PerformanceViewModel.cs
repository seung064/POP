using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POP_Project.Communication;
using POP_Project.Models;
using POP_Project.Repositories;
using POP_Project.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace POP_Project.ViewModels
{
    public partial class PerformanceViewModel : ObservableObject
    {
        private ModbusControl modbus = new ModbusControl();

        public PerformanceViewModel()
        {
            modbus.Connect();
        }

        [RelayCommand]
        private void NavigateBack()
        {
            // MainPage로 이동
            MainWindow.Instance.Navigate(new MainPage());
        }

        [ObservableProperty]
        private bool isStartChecked;
        [ObservableProperty]
        private bool isDefectChecked;

        // 컨베이어 시작 및 정지 속성
        partial void OnIsStartCheckedChanged(bool value)
        {
            _ = WriteCoilAsync(3, value);
        }

        // 불량 검출
        partial void OnIsDefectCheckedChanged(bool value)
        {
            _ = WriteCoilAsync(5, value);
        }

        // 비상 정지
        [RelayCommand]
        private async Task EmergencyStop()
        {
            await WriteCoilAsync(4, false); // 컨베이어 정지
        }

        [ObservableProperty]
        private string lampStatus;

        [ObservableProperty]
        private int conveyorSpeed;

        private async Task WriteCoilAsync(ushort coilAddress, bool value)
        {
            if (!modbus.IsConnected)
            {
                modbus.Connect();
            }

            if (modbus.IsConnected)
            {
                await modbus.WriteCoilAsync(coilAddress, value);
            }
        }
    }
}
