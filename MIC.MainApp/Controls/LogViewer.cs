using MIC.Infrastructure.Logging;
using NLog;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MIC.MainApp.Controls
{
    public partial class LogViewer : UserControl
    {
        private RichTextBox _richTextBox;
        private const int MaxLines = 500; // 最大显示行数，保护性能

        public LogViewer()
        {
            InitializeLogControl();
            // 订阅 NLog 自定义 Target 的事件
            LogEventTarget.OnLogReceived += LogEventTarget_OnLogReceived;
        }

        private void InitializeLogControl()
        {
            _richTextBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                BackColor = Color.Black,
                ForeColor = Color.White,
                Font = new Font("Consolas", 9),
                BorderStyle = BorderStyle.None
            };
            this.Controls.Add(_richTextBox);
        }

        private void LogEventTarget_OnLogReceived(LogEventInfo logInfo)
        {
            // 确保跨线程访问安全
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<LogEventInfo>(LogEventTarget_OnLogReceived), logInfo);
                return;
            }

            // 限制行数
            if (_richTextBox.Lines.Length > MaxLines)
            {
                _richTextBox.Select(0, _richTextBox.GetFirstCharIndexFromLine(100));
                _richTextBox.SelectedText = "";
            }

            // 根据等级设置颜色
            Color logColor = GetColorByLevel(logInfo.Level);

            _richTextBox.SelectionStart = _richTextBox.TextLength;
            _richTextBox.SelectionLength = 0;
            _richTextBox.SelectionColor = logColor;

            _richTextBox.AppendText($"{logInfo.TimeStamp:HH:mm:ss} [{logInfo.Level.Name.ToUpper()}] {logInfo.FormattedMessage}{Environment.NewLine}");

            // 自动滚动到底部
            _richTextBox.ScrollToCaret();
        }

        private Color GetColorByLevel(LogLevel level)
        {
            if (level == LogLevel.Error || level == LogLevel.Fatal) return Color.Red;
            if (level == LogLevel.Warn) return Color.Yellow;
            if (level == LogLevel.Debug) return Color.Gray;
            return Color.LimeGreen; // Info
        }

        // 释放资源时取消订阅
        protected override void OnHandleDestroyed(EventArgs e)
        {
            LogEventTarget.OnLogReceived -= LogEventTarget_OnLogReceived;
            base.OnHandleDestroyed(e);
        }
    }
}