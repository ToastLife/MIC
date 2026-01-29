using Newtonsoft.Json;
using System.IO;

namespace MIC.Infrastructure.Config
{
    /// <summary>
    /// JSON 配置文件辅助类。提供保存和加载 JSON 配置文件的通用方法。
    /// </summary>
    public static class JsonConfigHelper
    {
        /// <summary>
        /// 保存配置到指定文件路径。
        /// </summary>
        /// <typeparam name="T">配置数据的类型</typeparam>
        /// <param name="filePath">保存的文件路径</param>
        /// <param name="data">要保存的配置数据</param>
        public static void SaveConfig<T>(string filePath, T data)
        {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            string dir = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            File.WriteAllText(filePath, json);
        }

        /// <summary>
        /// 从指定文件路径加载配置。
        /// </summary>
        /// <typeparam name="T">配置数据的类型</typeparam>
        /// <param name="filePath">配置文件路径</param>
        /// <returns>加载的配置数据</returns>
        public static T LoadConfig<T>(string filePath)
        {
            if (!File.Exists(filePath)) return default(T);
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// 将对象序列化为 JSON 字符串。
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <param name="json">要序列化的对象</param>
        /// <returns>序列化后的 JSON 字符串</returns>
        public static string SerializeObject<T>(T json)
        {
            return JsonConvert.SerializeObject(json);
        }
    }

}
