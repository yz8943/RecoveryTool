# RecoveryTool

Windows 10/11 单机电脑恢复工具，基于 .NET 8 WPF。

## 发布

```powershell
dotnet publish .\RecoveryTool.csproj -c Release -r win-x64 --self-contained true --no-restore
```

发布结果位于 `bin\Release\net8.0-windows\win-x64\publish`，包含自包含的 `RecoveryTool.exe` 及配置、脚本和运行目录。

## 安全说明

重置系统和安装系统会造成数据丢失，程序要求管理员权限并在执行前进行二次确认。开发和编译过程不会自动执行这些操作。
