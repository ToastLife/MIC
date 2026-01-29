using System.Collections.Generic;

namespace MIC.Models.DTOs
{
    // 控件类型
    public enum WidgetType { ValueDisplay, StateIndicator, ActionButton, TrendChart }

    public class WidgetConfig
    {
        public string Name { get; set; }           // 控件名称
        public WidgetType Type { get; set; }       // 控件类型
        public int X { get; set; }                 // 坐标X
        public int Y { get; set; }                 // 坐标Y
        public int Width { get; set; }             // 宽度
        public int Height { get; set; }            // 高度
        public string DeviceId { get; set; }       // 绑定的设备ID
        public string Address { get; set; }        // 绑定的寄存器地址
        public string Format { get; set; }         // 显示格式（如 "0.00 ℃"）
        public string Text { get; set; }           // 静态文本（仅用于按钮/标签）
    }

    public class DashboardConfig
    {
        public string Title { get; set; }
        public int ColumnCount { get; set; }       // 网格列数（方便自动布局）
        public List<WidgetConfig> Widgets { get; set; } = new List<WidgetConfig>();
    }
}