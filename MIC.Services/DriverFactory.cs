using Microsoft.Extensions.DependencyInjection;
using MIC.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace MIC.Services
{
    public interface IDriverFactory
    {
        IDeviceDriver CreateDriver(string driverTypeName, string deviceId);
    }

    public class DriverFactory : IDriverFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<string, Type> _driverMap;

        public DriverFactory(IServiceProvider serviceProvider, Dictionary<string, Type> driverMap)
        {
            _serviceProvider = serviceProvider;
            _driverMap = driverMap;
        }

        public IDeviceDriver CreateDriver(string driverTypeName, string deviceId)
        {
            if (!_driverMap.TryGetValue(driverTypeName, out var type))
            {
                throw new Exception($"未找到驱动类型: {driverTypeName}");
            }

            // 利用 ActivatorUtilities 结合 DI 容器创建实例
            // 这样驱动类的构造函数里也可以注入 ILoggerService 等基础服务
            var driver = ActivatorUtilities.CreateInstance(_serviceProvider, type) as IDeviceDriver;
            if (driver != null)
            {
                driver.DeviceId = deviceId;
            }
            return driver;
        }
    }
}
