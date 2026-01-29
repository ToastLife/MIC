using MIC.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MIC.Services
{
    /// <summary>
    /// 插件加载器。负责从指定目录发现并加载实现了 IDeviceDriver 接口的驱动程序
    /// </summary>
    public class PluginLoader
    {
        private readonly ILoggerService _logger;

        /// <summary>
        /// 初始化插件加载器
        /// </summary>
        /// <param name="logger">日志服务</param>
        public PluginLoader(ILoggerService logger) => _logger = logger;

        /// <summary>
        /// 从指定目录扫描并发现所有驱动类。通过反射加载所有实现 IDeviceDriver 接口的类型
        /// </summary>
        /// <param name="pluginPath">插件所在目录路径（通常是 /bin/Debug/Plugins/）</param>
        /// <returns>驱动类型字典，Key 为类名，Value 为 Type 对象</returns>
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
                        // 使用类名作为 Key (例如 "ModbusTcpDriver")
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