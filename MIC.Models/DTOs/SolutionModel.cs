using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIC.Models.DTOs
{
    public class SolutionProject
    {
        public string Name { get; set; }
        public string Description { get; set; }
        // 方案关联的硬件配置
        public string DeviceConfigFile { get; set; } = "devices.json";
        // 方案关联的界面配置
        public string UIConfigFile { get; set; } = "dashboard.json";
        // 方案包含的多个流程文件名称列表
        public List<string> WorkflowFiles { get; set; } = new List<string>();
    }
}
