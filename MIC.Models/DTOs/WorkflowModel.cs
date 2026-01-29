using System.Collections.Generic;

namespace MIC.Models.DTOs
{
    // 步骤动作类型
    public enum ActionType { Write, WaitValue, Delay, Jump, Stop }

    public class WorkflowStep
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ActionType Action { get; set; }
        public string DeviceId { get; set; }
        public string Address { get; set; }
        public string TargetValue { get; set; } // 写入的值 或 等待的目标值
        public int TimeoutMs { get; set; }      // 仅用于 WaitValue
        public int NextStepId { get; set; }     // 正常下一步
        public int ErrorStepId { get; set; }    // 出错跳转步
    }

    public class WorkflowDefine
    {
        public string Guid { get; set; } = System.Guid.NewGuid().ToString();
        public string Name { get; set; }
        public List<WorkflowStep> Steps { get; set; } = new List<WorkflowStep>();
    }
}