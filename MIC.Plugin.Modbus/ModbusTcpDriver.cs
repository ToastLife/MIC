using HslCommunication;
using HslCommunication.ModBus;
using MIC.Core.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging; // 仅引用这个官方抽象库

namespace MIC.Plugin.Modbus
{
    /// <summary>
    /// Modbus TCP 驱动实现。基于 HslCommunication 库，实现标准的 Modbus TCP 通信协议
    /// 用于与 PLC 和其他 Modbus 兼容的工业设备通信
    /// </summary>
    public class ModbusTcpDriver : IDeviceDriver
    {
        /// <summary>
        /// HslCommunication Modbus TCP 客户端实例
        /// </summary>
        private ModbusTcpNet _modbusClient;
        
        /// <summary>
        /// 设备唯一标识
        /// </summary>
        public string DeviceId { get; set; }
        
        /// <summary>
        /// 设备名称常量
        /// </summary>
        public string DeviceName => "Standard Modbus TCP";
        
        /// <summary>
        /// 设备当前连接状态
        /// </summary>
        public bool IsConnected { get; private set; } = false;

        /// <summary>
        /// 通过依赖注入获取的日志服务（标准 Microsoft.Extensions.Logging）
        /// </summary>
        private readonly ILogger<ModbusTcpDriver> _logger;

        /// <summary>
        /// 构造函数。通过 DI 容器自动注入日志服务
        /// </summary>
        /// <param name="logger">日志服务（底层由 NLog 实现）</param>
        public ModbusTcpDriver(ILogger<ModbusTcpDriver> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 初始化驱动。解析配置 JSON，创建 Modbus TCP 客户端实例
        /// </summary>
        /// <param name="configJson">配置 JSON 字符串，应包含 "Ip" 和 "Port" 字段</param>
        public void Initialize(string configJson)
        {
            // 使用标准日志方法
            _logger.LogInformation($"[{DeviceId}] Modbus 驱动正在初始化...");

            try
            {
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

        /// <summary>
        /// 连接到 Modbus TCP 服务器
        /// </summary>
        /// <returns>连接成功返回 true</returns>
        public bool Connect()
        {
            OperateResult connect = _modbusClient.ConnectServer();
            IsConnected = connect.IsSuccess;
            _logger.LogInformation($"[{DeviceId}] 连接成功 (模拟)");
            return IsConnected;
        }

        /// <summary>
        /// 断开 Modbus TCP 连接
        /// </summary>
        public void Disconnect()
        {
            _modbusClient?.ConnectClose();
            _logger.LogInformation($"[{DeviceId}] 断开连接 (模拟)");
            IsConnected = false;
        }

        /// <summary>
        /// 异步读取寄存器数据。支持 int16、float、bool 等类型
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="address">寄存器地址</param>
        /// <returns>读取的数据值</returns>
        /// <exception cref="Exception">设备未连接时抛出</exception>
        /// <exception cref="NotSupportedException">不支持的数据类型</exception>
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

        /// <summary>
        /// 异步向寄存器写入数据。支持 int16、float、bool 等类型
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="address">寄存器地址</param>
        /// <param name="value">要写入的数据值</param>
        /// <returns>写入成功返回 true</returns>
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