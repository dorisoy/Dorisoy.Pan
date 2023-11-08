namespace Dorisoy.PanClient.Utils;

/// <summary>
/// 获取应用程序基本信息
/// </summary>
public interface IApplicationInfo
{
    string Name { get; }

    string Organization { get; }

    string Copyright { get; }

    string Version { get; }

    string FileVersion { get; }

    DateTime Created { get; }
}
