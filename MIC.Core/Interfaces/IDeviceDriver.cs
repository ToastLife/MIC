using System;
using System.Threading.Tasks;

namespace MIC.Core.Interfaces
{
    // 定义所有设备插件必须遵循的接口
    public interface IDeviceDriver
    {
        string DeviceId { get; set; }
        string DeviceName { get; }
        bool IsConnected { get; }

        // 初始化 (传入连接字符串或配置对象)
        void Initialize(string configJson);

        // 连接
        bool Connect();

        // 断开
        void Disconnect();

        // 读取数据 (泛型，支持读取不同类型)
        Task<T> ReadAsync<T>(string address);

        // 写入数据
        Task<bool> WriteAsync<T>(string address, T value);
    }
}