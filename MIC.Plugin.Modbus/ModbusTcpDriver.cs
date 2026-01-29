using HslCommunication;
using HslCommunication.ModBus;
using MIC.Core.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging; // 仅引用这个官方抽象库

namespace MIC.Plugin.Modbus
{
    public class ModbusTcpDriver : IDeviceDriver
    {
        private ModbusTcpNet _modbusClient;
        public string DeviceId { get; set; }
        public string DeviceName => "Standard Modbus TCP";
        public bool IsConnected { get; private set; } = false;

        // 使用标准泛型接口，泛型参数会自动标记日志来源为 "MIC.Plugin.Modbus.ModbusTcpDriver"
        private readonly ILogger<ModbusTcpDriver> _logger;

        // DI 容器会自动注入这个 logger，底层其实是 NLog，但插件不知道也不关心
        public ModbusTcpDriver(ILogger<ModbusTcpDriver> logger)
        {
            _logger = logger;
        }

        public void Initialize(string configJson)
        {
            // 使用标准日志方法
            _logger.LogInformation($"[{DeviceId}] Modbus 驱动正在初始化...");

            try
            {
                // 模拟解析逻辑
                // 解析配置：假设 configJson 包含 IpAddress 和 Port
                var config = JObject.Parse(configJson);
                string ip = config["Ip"]?.ToString() ?? "127.0.0.1";
                int port = int.Parse(config["Port"]?.ToString() ?? "502");

                _modbusClient = new ModbusTcpNet(ip, port);
                _logger.LogDebug($"配置参数: {configJson}");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, $"[{DeviceId}] 初始化出错");
            }
        }

        public bool Connect()
        {
            OperateResult connect = _modbusClient.ConnectServer();
            IsConnected = connect.IsSuccess;
            _logger.LogInformation($"[{DeviceId}] 连接成功 (模拟)");
            return IsConnected;
        }

        public void Disconnect()
        {
            _modbusClient?.ConnectClose();
            _logger.LogInformation($"[{DeviceId}] 断开连接 (模拟)");
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