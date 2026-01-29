using MIC.Models.DTOs;
using MIC.Core.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace MIC.Services
{
    /// <summary>
    /// 设备管理器。负责设备的创建、连接、断开和生命周期管理。
    /// 采用 ConcurrentDictionary 以支持多线程安全操作
    /// </summary>
    public class DeviceManager
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILoggerService _logger;
        /// <summary>
        /// 线程安全的设备字典。Key: 设备ID，Value: 设备驱动实例
        /// </summary>
        private ConcurrentDictionary<string, IDeviceDriver> _devices = new ConcurrentDictionary<string, IDeviceDriver>();

        /// <summary>
        /// 初始化设备管理器
        /// </summary>
        /// <param name="serviceProvider">DI 容器，用于动态解析设备驱动</param>
        /// <param name="logger">日志服务</param>
        public DeviceManager(IServiceProvider serviceProvider, ILoggerService logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        /// <summary>
        /// 添加一个设备到管理器中
        /// </summary>
        /// <param name="driver">设备驱动实例</param>
        public void AddDevice(IDeviceDriver driver)
        {
            if (_devices.TryAdd(driver.DeviceId, driver))
            {
                _logger.Info($"Device added: {driver.DeviceId}");
            }
        }

        /// <summary>
        /// 根据设备ID获取设备实例
        /// </summary>
        /// <param name="deviceId">设备ID</param>
        /// <returns>设备驱动实例，如果不存在则返回 null</returns>
        public IDeviceDriver GetDevice(string deviceId)
        {
            if (_devices.TryGetValue(deviceId, out var driver))
            {
                return driver;
            }
            return null;
        }

        /// <summary>
        /// 获取所有已管理的设备
        /// </summary>
        /// <returns>所有设备驱动实例的集合</returns>
        public IEnumerable<IDeviceDriver> GetAllDevices()
        {
            return _devices.Values;
        }

        /// <summary>
        /// 异步连接所有设备。为每个设备创建单独的 Task 以实现并行连接
        /// </summary>
        public async Task ConnectAllAsync()
        {
            var tasks = new List<Task>();
            foreach (var device in _devices.Values)
            {
                tasks.Add(Task.Run(() => device.Connect()));
            }
            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// 动态创建设备驱动实例并添加到管理器中
        /// </summary>
        /// <param name="driverTypeName">驱动类型名称（如 "ModbusTcpDriver"）</param>
        /// <param name="deviceId">设备 ID</param>
        public void CreateAndAddDevice(string driverTypeName, string deviceId)
        {
            var driverType = Type.GetType(driverTypeName);
            if (driverType == null || !typeof(IDeviceDriver).IsAssignableFrom(driverType))
            {
                throw new ArgumentException($"Invalid driver type: {driverTypeName}");
            }

            var driver = (IDeviceDriver)_serviceProvider.GetRequiredService(driverType);
            driver.DeviceId = deviceId;
            AddDevice(driver);
        }

        /// <summary>
        /// 断开所有设备的连接
        /// </summary>
        public void DisconnectAll()
        {
            foreach (var device in _devices.Values)
            {
                if (device.IsConnected)
                {
                    device.Disconnect();
                    _logger.Info($"Device disconnected: {device.DeviceId}");
                }
            }
        }
    }
}