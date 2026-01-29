# 项目说明文档

## 工程目录结构
```
MIC
├── MIC.Core
│   ├── Interfaces
│   │   ├── ILoggerService.cs  // 日志服务接口
│   │   ├── ILogQueryService.cs  // 日志查询接口
│   │   └── IDeviceDriver.cs  // 设备驱动接口
│   ├── Entities
│   │   └── SystemLog.cs  // 系统日志实体
│   ├── Constants
│   │   └── GlobalConstants.cs  // 全局常量定义
│   └── Properties
│       └── AssemblyInfo.cs  // 程序集信息
├── MIC.Infrastructure
│   ├── Logging
│   │   ├── LogEventTarget.cs  // 日志事件目标
│   │   └── NLogLogger.cs  // NLog 日志实现
│   ├── Services
│   │   └── LogQueryService.cs  // 日志查询服务
│   ├── Database
│   │   └── SqliteDapperHelper.cs  // SQLite 数据库辅助类
│   ├── Config
│   │   └── JsonConfigHelper.cs  // JSON 配置文件辅助类
│   └── Properties
│       └── AssemblyInfo.cs  // 程序集信息
├── MIC.MainApp
│   ├── Forms
│   │   ├── MainForm.cs  // 主窗体逻辑
│   │   ├── SolutionMgrForm.cs  // 方案管理窗体
│   │   └── WorkflowEditorForm.cs  // 工作流编辑窗体
│   ├── Controls
│   │   └── LogViewer.cs  // 日志查看控件
│   ├── Program.cs  // 应用程序入口
│   ├── GlobalContext.cs  // 全局上下文
│   └── Properties
│       └── AssemblyInfo.cs  // 程序集信息
├── MIC.Models
│   ├── DTOs
│   │   ├── SolutionModel.cs  // 方案模型
│   │   ├── WorkflowModel.cs  // 工作流模型
│   │   └── SystemSettings.cs  // 系统设置模型
│   ├── Entities
│   │   ├── User.cs  // 用户实体
│   │   ├── Alarm.cs  // 告警实体
│   │   └── HistoricalData.cs  // 历史数据实体
│   ├── Enums
│   │   └── SystemEnums.cs  // 系统枚举
│   └── Properties
│       └── AssemblyInfo.cs  // 程序集信息
├── MIC.Services
│   ├── ProjectManager.cs  // 项目管理器
│   ├── DeviceManager.cs  // 设备管理器
│   ├── WorkflowEngine.cs  // 工作流引擎
│   ├── PluginLoader.cs  // 插件加载器
│   ├── PollingService.cs  // 轮询服务
│   ├── DataService.cs  // 数据服务
│   ├── AuthService.cs  // 认证服务
│   └── Properties
│       └── AssemblyInfo.cs  // 程序集信息
├── MIC.Plugin.MES
│   └── Class1.cs  // MES 插件示例
├── MIC.Plugin.Modbus
│   └── ModbusTcpDriver.cs  // Modbus TCP 驱动实现
└── MIC.Plugin.Vision
    └── Class1.cs  // 视觉插件示例
```

## 项目间的关系
- **MIC.Core**：定义核心接口和实体，供其他项目引用。
- **MIC.Infrastructure**：提供基础设施服务（如日志、数据库操作）。
- **MIC.MainApp**：主应用程序，包含 UI 和全局上下文。
- **MIC.Models**：定义数据传输对象（DTO）和实体类。
- **MIC.Services**：实现业务逻辑，包括项目管理、设备管理、工作流引擎等。
- **MIC.Plugin.MES**、**MIC.Plugin.Modbus**、**MIC.Plugin.Vision**：插件项目，扩展系统功能。

## 每个代码文件的说明
- **MIC.Core.Interfaces**：定义了日志服务接口（ILoggerService）、设备驱动接口（IDeviceDriver）等。
- **MIC.Infrastructure.Logging.NLogLogger**：基于 NLog 实现的日志服务。
- **MIC.Services.ProjectManager**：管理项目的加载、保存和工作流操作。
- **MIC.Services.WorkflowEngine**：执行工作流的核心引擎。
- **MIC.Plugin.Modbus.ModbusTcpDriver**：实现 Modbus TCP 协议的设备驱动。

## 如何进行开发
1. **环境配置**：
   - 确保安装了 Visual Studio 2019 或更高版本。
   - 安装 .NET Framework 4.8 开发工具。
2. **运行项目**：
   - 打开解决方案文件 `MIC.sln`。
   - 设置 `MIC.MainApp` 为启动项目。
   - 按下 `F5` 运行项目。
3. **开发新功能**：
   - 在 `MIC.Services` 中添加新的业务逻辑。
   - 在 `MIC.Plugin.*` 中开发新的插件。
4. **测试**：
   - 使用 Visual Studio 的测试工具运行单元测试。
   - 确保所有测试通过后提交代码。
5. **部署**：
   - 使用发布配置生成可执行文件。
   - 将生成的文件夹部署到目标环境。

---

如需进一步帮助，请随时联系开发团队。