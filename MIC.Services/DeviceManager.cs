using MIC.Core.Interfaces;
using MIC.Infrastructure.Config;
using MIC.Models.DTOs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MIC.Services
{
    public class DeviceManager
    {
        private readonly IDriverFactory _driverFactory;
        private readonly ILoggerService _logger;
        // 确保使用 ConcurrentDictionary 以支持多线程安全
        private ConcurrentDictionary<string, IDeviceDriver> _devices = new ConcurrentDictionary<string, IDeviceDriver>();

        public DeviceManager(IDriverFactory driverFactory, ILoggerService logger)
        {
            _driverFactory = driverFactory;
            _logger = logger;
        }

        public void AddDevice(IDeviceDriver driver)
        {
            if (_devices.TryAdd(driver.DeviceId, driver))
            {
                _logger.Info($"Device added: {driver.DeviceId}");
            }
        }

        public IDeviceDriver GetDevice(string deviceId)
        {
            if (_devices.TryGetValue(deviceId, out var driver))
            {
                return driver;
            }
            return null;
        }

        // 修复3：实现 GetAllDevices 方法
        public IEnumerable<IDeviceDriver> GetAllDevices()
        {
            return _devices.Values;
        }

        public async Task ConnectAllAsync()
        {
            foreach (var device in _devices.Values)
            {
                await Task.Run(() =>
                {
                    try
                    {
                        if (device.Connect())
                            _logger.Info($"Device {device.DeviceId} connected.");
                        else
                            _logger.Error($"Device {device.DeviceId} connection failed.");
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"Error connecting {device.DeviceId}", ex);
                    }
                });
            }
        }

        public void DisconnectAll()
        {
            foreach (var device in _devices.Values)
            {
                try { device.Disconnect(); } catch { }
            }
        }

        //  注意 ReloadConfig 中的实例化逻辑通常需要结合反射或工厂模式，我提供了一个框架，你需要确保 JSON 解析出的配置能正确转换为 IDeviceDriver 实例。
        // 修复4：实现 ReloadConfig 方法
        // 参数 path: devices.json 的路径
        // 参数 availablePlugins: 插件加载器已加载的驱动列表（可选，用于反射创建实例）
        public void ReloadConfig(string configPath)
        {
            DisconnectAll();
            _devices.Clear();

            if (!File.Exists(configPath)) return;

            // 加载方案对应的 devices.json
            var configs = JsonConfigHelper.LoadConfig<List<DeviceConfig>>(configPath);

            foreach (var cfg in configs)
            {
                try
                {
                    // 通过工厂创建具体驱动实例
                    var driver = _driverFactory.CreateDriver(cfg.DriverType, cfg.DeviceId);

                    // 将 JSON 配置传给驱动进行内部初始化 (地址、端口等)
                    string driverJson = JsonConfigHelper.SerializeObject(cfg);
                    driver.Initialize(driverJson);

                    _devices.TryAdd(cfg.DeviceId, driver);
                    _logger.Info($"设备 [{cfg.DeviceId}] 已挂载，驱动: {cfg.DriverType}");
                }
                catch (Exception ex)
                {
                    _logger.Error($"初始化设备 {cfg.DeviceId} 失败", ex);
                }
            }
        }

    }

    // 补充：简单的配置类定义，用于 ReloadConfig 解析 JSON
    public class DeviceConfig
    {
        public string DeviceId { get; set; }
        public string DriverType { get; set; } // 例如 "ModbusTcp"
        public string IpAddress { get; set; }
        public int Port { get; set; }
    }
}