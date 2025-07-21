using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using POP_Project.Communication;
using POP_Project.Models;
using POP_Project.Repositories;
using POP_Project.Views;
using System.IO.Ports;

namespace POP_Project.ViewModels
{
    public partial class PerformanceViewModel : ObservableObject
    {
        private ModbusControl modbus = new ModbusControl();
        private SerialPort serialPort;

        public PerformanceViewModel()
        {
           modbus.Connect(); // Modbus 연결
           serialPort.Open();
        }

        [RelayCommand]
        private void NavigateBack()
        {
            // MainPage로 이동
            MainWindow.Instance.Navigate(new MainPage());
        }

        [RelayCommand]
        private void EmergencyStop()
        {
            modbus.WriteCoilAsync(0, false); // 컨베이어 정지
            modbus.WriteCoilAsync(1, false); // 램프 OFF

            IsStartChecked = false;
            IsDefectChecked = false;
            LampStatus = "OFF";
            ConveyorSpeed = 0;
        }

        // 컨베이어 시작 및 정지 속성
        [ObservableProperty]
        private bool isStartChecked;
        partial void OnIsStartCheckedChanged(bool value)
        {
            ControlConveyor(value);
        }

        // 불량 검출
        [ObservableProperty]
        private bool isDefectChecked;
        partial void OnIsDefectCheckedChanged(bool value)
        {
            ControlLamp(value);
        }

        [ObservableProperty]
        private string lampStatus;

        [ObservableProperty]
        private int conveyorSpeed;

        private async void ControlConveyor(bool isOn)
        {
            await modbus.WriteCoilAsync(0, isOn); // 예: Coil 0 - 컨베이어
            ConveyorSpeed = isOn ? 100 : 0;
        }

        private async void ControlLamp(bool isOn)
        {
            await modbus.WriteCoilAsync(1, isOn); // 예: Coil 1 - 램프
            LampStatus = isOn ? "ON" : "OFF";
        }
    }
}
