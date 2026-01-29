using System;
using System.Threading.Tasks;

namespace MIC.Core.Interfaces
{
    /// <summary>
    /// 设备驱动接口。定义所有设备插件必须遵循的契约。任何设备驱动都必须实现这个接口
    /// </summary>
    public interface IDeviceDriver
    {
        /// <summary>
        /// 设备唯一标识
        /// </summary>
        string DeviceId { get; set; }
        
        /// <summary>
        /// 设备名称（描述性信息，例如 "Modbus TCP"）
        /// </summary>
        string DeviceName { get; }
        
        /// <summary>
        /// 设备连接状态
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// 初始化设备。传入连接参数（通常是 JSON 格式的配置字符串）
        /// </summary>
        /// <param name="configJson">设备配置信息，通常包含 IP、端口、地址等</param>
        void Initialize(string configJson);

        /// <summary>
        /// 连接到设备。建立通信链接
        /// </summary>
        /// <returns>连接成功返回 true</returns>
        bool Connect();

        /// <summary>
        /// 断开与设备的连接
        /// </summary>
        void Disconnect();

        /// <summary>
        /// 异步读取设备数据。支持泛型以读取不同数据类型
        /// </summary>
        /// <typeparam name="T">数据类型（例如 int、float、bool）</typeparam>
        /// <param name="address">寄存器或地址标识</param>
        /// <returns>读取到的数据值</returns>
        Task<T> ReadAsync<T>(string address);

        /// <summary>
        /// 异步向设备写入数据。支持泛型以写入不同数据类型
        /// </summary>
        /// <typeparam name="T">数据类型（例如 int、float、bool）</typeparam>
        /// <param name="address">寄存器或地址标识</param>
        /// <param name="value">要写入的数据值</param>
        /// <returns>写入成功返回 true</returns>
        Task<bool> WriteAsync<T>(string address, T value);
    }
}