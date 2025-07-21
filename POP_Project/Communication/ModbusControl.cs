using Modbus.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace POP_Project.Communication
{
    
    public class ModbusControl
    {
        // modbus 통신에 필요한 TcpClient, Modbus 마스터 객체
        private TcpClient tcpClient;
        private ModbusIpMaster master;
        // PLC IP 주소와 포트 번호
        private string ipAddress = "192.168.0.10"; // PLC IP 주소
        private int port = 502;
        // 현재 연결 상태를 확인하는 속성
        public bool IsConnected { get; private set; } = false;

        

        // Modbus 서버(PLC)에 연결하는 메서드
        public void Connect()
        {
            try
            {
                // TcpClient 객체 초기화 및 PLC에 연결 시도
                tcpClient = new TcpClient();
                tcpClient.Connect(ipAddress,port);

                // 연결된 TcpClient로부터 Modbus 마스터 생성
                master = ModbusIpMaster.CreateIp(tcpClient);

                // 연결 성공 시 상태 갱신
                IsConnected = true;
            }
            catch (Exception ex)
            {
                // 예외 처리
                IsConnected = false;
                MessageBox.Show($"Modbus 연결 실패: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Modbus 서버 연결 해제 메서드
        public void Disconnect()
        {
            // 리소스
            master?.Dispose();
            tcpClient?.Close();
            IsConnected = false;
        }

        // 비동기로 Coil 값을 읽는 메서드
        public async Task<bool> ReadCoilAsync(ushort address)
        {
            if (!IsConnected) return false;
            var result = await master.ReadCoilsAsync(address, 1);
            return result[0];
        }

        // 비동기로 Coil 값을 쓰는 메서드
        public async Task WriteCoilAsync(ushort address, bool value)
        {
            if (!IsConnected) return;
            await master.WriteSingleCoilAsync(address, value);
        }

        private async Task ControlMultiplePlcLamps()
        {
            var modbus = new ModbusControl();
            modbus.Connect();

            if (!modbus.IsConnected)
                return;

            try
            {
                await modbus.WriteCoilAsync(3, true);

                await modbus.WriteCoilAsync(4, true);

                await modbus.WriteCoilAsync(5, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Modbus 제어 중 오류 발생: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                modbus.Disconnect();
            }
        }
    }
}
