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
using System.Windows;
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
        // 불량 검출시 코일 동작을 위한 속성
        [ObservableProperty]
        private Product currentProduct;

        [ObservableProperty]
        private bool isStartChecked;
        [ObservableProperty]
        private bool isDefectChecked;
        [ObservableProperty]
        private bool isEmergencyStopChecked;

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

        // 실시간 반영
        partial void OnCurrentProductChanged(Product value)
        {
            _ = CheckProductDefectAsync();
        }

        public async Task CheckProductDefectAsync()
        {

            // 1. 제품의 불량 여부 확인 (Defective_or_not == true)
            if (CurrentProduct?.Defective_or_not == true)
            {
                try
                {
                    // 2. PLC 코일 5번을 ON
                    await modbus.WriteCoilAsync(5, true);

                    // 3. 7초 대기
                    await Task.Delay(6000);

                    // 4. 6초 후 PLC 코일 5번을 OFF
                    await modbus.WriteCoilAsync(5, false);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"PLC 제어 실패 (불량 검출): {ex.Message}", "Modbus 통신 오류", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                // 정상 제품인 경우
                Debug.WriteLine($"정상 제품입니다 ({CurrentProduct.QR_Code}). 별도 조치 없음.");
            }
        }

        // 비상 정지
        partial void OnIsEmergencyStopCheckedChanged(bool value)
        {
            _ = WriteCoilAsync(4, value);
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