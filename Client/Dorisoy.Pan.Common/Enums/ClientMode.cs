using System.ComponentModel;

namespace Dorisoy.Pan.Common;


/// <summary>
/// 客户端模式
/// </summary>
public enum ClientMode
{
    /// <summary>
    /// 医疗机构
    /// </summary>
    [Description("医疗机构版")]
    Hospital,
    /// <summary>
    /// 院校机构
    /// </summary>
    [Description("教育院校版")]
    Academy
}
