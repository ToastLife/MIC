using Newtonsoft.Json;
using System.IO;

namespace MIC.Infrastructure.Config
{
    public static class JsonConfigHelper
    {
        // 泛型保存方法，解决 CS1503
        public static void SaveConfig<T>(string filePath, T data)
        {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            string dir = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            File.WriteAllText(filePath, json);
        }

        public static T LoadConfig<T>(string filePath)
        {
            if (!File.Exists(filePath)) return default(T);
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static string SerializeObject<T>(T json)
        {
            return JsonConvert.SerializeObject(json);
        }
    }

}
