﻿using Dorisoy.PanClient.Core;

namespace Dorisoy.PanClient.Utils;

/// <summary>
/// 获取应用程序基本信息
/// </summary>
public class ApplicationInfo : IApplicationInfo
{
    public string Name { get; }
    public string Organization { get; }
    public string Copyright { get; }
    public string Version { get; }
    public string FileVersion { get; }
    public DateTime Created { get; }

    public ApplicationInfo(Assembly assembly)
    {
        Name = assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product ?? "Dorisoy.PanClient";
        Organization = assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company ?? "";
        Copyright = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright ?? "";
        Version = assembly.GetName()?.Version.ToString();
        FileVersion = assembly.GetName()?.Version.ToString();
        Created = DateTime.Now;
    }
}



