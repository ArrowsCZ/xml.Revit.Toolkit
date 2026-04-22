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

namespace xml.Revit.Toolkit.Attributes
{
    /// <summary>
    /// 未开发完成的功能或不想自动添加到面板时可使用此特性
    /// </summary>
    /// <remarks>
    /// 示例
    /// <br>[<see cref="XmlUnfinishedAttribute"/>]</br>
    /// <br>public class Cmd : <see cref="Utils.XmlExternalCommand"/></br>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public sealed class XmlUnfinishedAttribute : Attribute { }
}
