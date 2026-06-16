namespace AutoScrew.Application.Configuration;

/// <summary>产线拧紧控制路径：上位机逐钉换参 vs 设备顺序程序。</summary>
public enum ProductionTighteningMode
{
    HostGuided = 0,
    DeviceProgram = 1,
}
