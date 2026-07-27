# RecoveryTool

Windows 10/11 单机电脑恢复工具，基于 .NET 8 WPF。

## 发布

```powershell
dotnet publish .\RecoveryTool.csproj -c Release -r win-x64 --self-contained true
```

发布结果位于 `bin\Release\net8.0-windows\win-x64\publish`，默认只包含一个自包含的 `RecoveryTool.exe`。程序首次运行时会在 EXE 同目录创建 `Logs` 和 `ISO` 目录。

## 安全说明

重置系统和安装系统会造成数据丢失，程序要求管理员权限并在执行前进行二次确认。开发和编译过程不会自动执行这些操作。
