using System;

namespace MIC.Models.DTOs
{
    public class WorkflowEventArgs : EventArgs
    {
        public int StepId { get; set; }     // 当前步骤ID
        public string Message { get; set; } // 步骤名称或消息
        public string Status { get; set; }  // 状态：Running, Success, Error, Timeout
    }
}