using MIC.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MIC.Services
{
    public class PluginLoader
    {
        private readonly ILoggerService _logger;

        public PluginLoader(ILoggerService logger) => _logger = logger;

        public Dictionary<string, Type> DiscoverDrivers(string pluginPath)
        {
            var driverTypes = new Dictionary<string, Type>();

            if (!Directory.Exists(pluginPath)) return driverTypes;

            var dlls = Directory.GetFiles(pluginPath, "*.dll");
            foreach (var dll in dlls)
            {
                try
                {
                    var assembly = Assembly.LoadFrom(dll);
                    var types = assembly.GetTypes()
                        .Where(t => typeof(IDeviceDriver).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                    foreach (var type in types)
                    {
                        // 使用类名或自定义特性作为 Key (例如 "ModbusTcpDriver")
                        driverTypes.Add(type.Name, type);
                        _logger.Info($"发现插件驱动: {type.Name}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error($"加载插件失败: {dll}", ex);
                }
            }
            return driverTypes;
        }
    }
}