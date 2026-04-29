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

namespace xml.Revit.Toolkit;

/// <summary>
/// 测试功能
/// </summary>
[Xml("测试", "请输入功能提示")]
[Transaction(TransactionMode.Manual)]
public sealed class Cmd : XmlExternalCommand
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="message"></param>
    /// <param name="elements"></param>
    protected override void Execute(ref string message, ElementSet elements) => XmlDoc.Print(uidoc.Document.Title);
}
