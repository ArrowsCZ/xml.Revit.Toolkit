/* 作    者: xml
** 创建时间: 2024/6/23 12:39:52
**
** Copyright 2024 by zedmoster
** Permission to use, copy, modify, and distribute this software in
** object code form for any purpose and without fee is hereby granted,
** provided that the above copyright notice appears in all copies and
** that both that copyright notice and the limited warranty and
** restricted rights notice below appear in all supporting
** documentation.
*/

namespace xml.Revit.Toolkit.Extensions;

/// <summary>
/// Revit Object Extensions
/// </summary>
public static class RevitExtensions
{
    /// <summary>
    /// Throw null exception if <see cref="Autodesk.Revit.DB.Element"/> is null or invalid
    /// </summary>
    /// <param name="element"></param>
    /// <exception cref="System.ArgumentNullException"></exception>
    public static void ThrowIfNullOrInvalid(this Element element)
    {
        if (element is null)
            throw new ArgumentNullException(nameof(element));
        if (!element.IsValidObject)
            throw new ArgumentNullException(nameof(element), "In Revit Element must be valid object");
    }

    /// <summary>
    /// Throw null exception if <see cref="Autodesk.Revit.DB.ElementId"/> is null or invalidElementId
    /// </summary>
    /// <param name="elementId"></param>
    /// <exception cref="System.ArgumentNullException"></exception>
    public static void ThrowIfNullOrInvalid(this ElementId elementId)
    {
        if (elementId is null)
            throw new ArgumentNullException(nameof(elementId));
        if (elementId == ElementId.InvalidElementId)
            throw new ArgumentNullException(nameof(elementId), "In Revit ElementId is ElementId.InvalidElementId");
    }

    /// <summary>
    /// Throw null exception if <see cref="Document"/> is null or invalid
    /// </summary>
    /// <param name="doc"></param>
    /// <exception cref="System.ArgumentNullException"></exception>
    public static void ThrowIfNullOrInvalid(this Document doc)
    {
        if (doc is null)
            throw new ArgumentNullException(nameof(doc));
        if (!doc.IsValidObject)
            throw new ArgumentNullException(nameof(doc), "In Revit Document must be valid object");
    }
}
