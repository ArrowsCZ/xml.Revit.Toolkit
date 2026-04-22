/* 作    者: xml
** 创建时间: 2024/2/16 20:26:06
**
** Copyright 2024 by zedmoster
** Permission to use, copy, modify, and distribute this software in
** object code form for any purpose and without fee is hereby granted,
** provided that the above copyright notice appears in all copies and
** that both that copyright notice and the limited warranty and
** restricted rights notice below appear in all supporting
** documentation.
*/

namespace xml.Revit.Toolkit.Attributes;

/// <summary>
/// 添加此特性的功能会自动添加到面板
/// </summary>
/// <remarks>
/// 示例
/// <br>[<see cref="XmlAttribute"/>("功能名称", "功能用途提示基本信息")]</br>
/// <br>[<see cref="TransactionAttribute"/>(<see cref="TransactionMode.Manual"/>)]</br>
/// <br>public class Cmd : <see cref="Utils.XmlExternalCommand"/></br>
/// </remarks>
/// <remarks>
/// 功能
/// </remarks>
/// <param name="name">功能名称</param>
/// <param name="tooltip">功能提示</param>
/// <param name="description">长按提示</param>
[AttributeUsage(AttributeTargets.Class)]
public sealed class XmlAttribute(string name, string tooltip = null, string description = null)
    : Attribute
{
    /// <summary>
    /// 作者
    /// </summary>
    internal const string Author = "广州明周科技有限公司";

    /// <summary>
    /// 微信公众号
    /// </summary>
    const string Url = "微信公众号:Revit二次开发教程";

    /// <summary>
    /// 长按提示
    /// </summary>
    const string Tip = "关注公众号获取最新功能消息(F1获取帮助)";

    /// <summary>
    /// 功能名称
    /// </summary>
    public string Name { get; set; } = name;

    /// <summary>
    /// 功能提示
    /// </summary>
    public string ToolTip { get; set; } = tooltip ?? Url;

    /// <summary>
    /// 功能长按提示
    /// </summary>
    public string Description { get; set; } = description ?? Tip;
}
