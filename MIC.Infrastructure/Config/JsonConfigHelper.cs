using Newtonsoft.Json;
using System;
using System.IO;

namespace MIC.Infrastructure.Config
{
    /// <summary>
    /// JsonConfigHelper 已弃用，请使用 Microsoft.Extensions.Configuration 替代。
    /// </summary>
    [Obsolete("Use Microsoft.Extensions.Configuration instead.")]
    public static class JsonConfigHelper
    {
        // 保留方法以兼容旧代码，但标记为过时
        [Obsolete("Use IConfiguration for loading configurations.")]
        public static void SaveConfig<T>(string filePath, T data)
        {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            string dir = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            File.WriteAllText(filePath, json);
        }

        [Obsolete("Use IConfiguration for loading configurations.")]
        public static T LoadConfig<T>(string filePath)
        {
            if (!File.Exists(filePath)) return default(T);
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }

}
