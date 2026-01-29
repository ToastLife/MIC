using HslCommunication;
using HslCommunication.ModBus;
using MIC.Core.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MIC.Plugin.Modbus
{
    public class ModbusTcpDriver : IDeviceDriver
    {
        private ModbusTcpNet _modbusClient;
        public string DeviceId { get; set; }
        public string DeviceName => "Standard Modbus TCP";
        public bool IsConnected { get; private set; } = false;

        public void Initialize(string configJson)
        {
            // 解析配置：假设 configJson 包含 IpAddress 和 Port
            var config = JObject.Parse(configJson);
            string ip = config["Ip"]?.ToString() ?? "127.0.0.1";
            int port = int.Parse(config["Port"]?.ToString() ?? "502");

            _modbusClient = new ModbusTcpNet(ip, port);
        }

        public bool Connect()
        {
            OperateResult connect = _modbusClient.ConnectServer();
            IsConnected = connect.IsSuccess;
            return IsConnected;
        }

        public void Disconnect()
        {
            _modbusClient?.ConnectClose();
            IsConnected = false;
        }

        public async Task<T> ReadAsync<T>(string address)
        {
            if (!IsConnected) throw new Exception("Device not connected");

            // 简单演示读取 Int16，实际需根据 T 类型做 switch case 处理 Hsl 的不同读取方法
            if (typeof(T) == typeof(short))
            {
                var result = await _modbusClient.ReadInt16Async(address);
                if (result.IsSuccess) return (T)(object)result.Content;
            }
            // ... 处理 float, bool 等

            throw new NotSupportedException($"Type {typeof(T)} not supported yet.");
        }

        public async Task<bool> WriteAsync<T>(string address, T value)
        {
            if (!IsConnected) return false;

            OperateResult result;

            // HslCommunication 的原生支持
            if (typeof(T) == typeof(short))
                result = await _modbusClient.WriteAsync(address, Convert.ToInt16(value));
            else if (typeof(T) == typeof(float))
                result = await _modbusClient.WriteAsync(address, Convert.ToSingle(value));
            else if (typeof(T) == typeof(bool))
                result = await _modbusClient.WriteAsync(address, Convert.ToBoolean(value));
            else
                result = await _modbusClient.WriteAsync(address, value.ToString());

            return result.IsSuccess;
        }
 
    }
}