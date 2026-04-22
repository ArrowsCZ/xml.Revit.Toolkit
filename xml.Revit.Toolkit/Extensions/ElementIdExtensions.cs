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

namespace xml.Revit.Toolkit.Extensions;

/// <summary>
/// ElementIdExtensions
/// </summary>
public static class ElementIdExtensions
{
    /// <param name="elementIds">ElementIds 集合。</param>
    extension(IEnumerable<ElementId> elementIds)
    {
        /// <summary>
        /// 将 ElementIds 集合转换为 Element 列表。
        /// </summary>
        /// <param name="doc">文档对象。</param>
        /// <returns>Element 对象列表。</returns>
        [Pure]
        public List<Element> ToElements(Document doc)
        {
            return elementIds.Select(id => id.ToElement(doc)).Where(el => el != null).ToList();
        }

        /// <summary>
        /// 将 ElementIds 集合转换为元素列表。
        /// </summary>
        /// <typeparam name="T">元素类型。</typeparam>
        /// <param name="doc">文档对象。</param>
        /// <returns>元素列表。</returns>
        [Pure]
        public List<T> ToElements<T>(Document doc)
            where T : Element
        {
            return elementIds.Select(id => id.ToElement(doc)).OfType<T>().ToList();
        }
    }

    /// <summary>
    /// 将元素列表转换为 ElementId 列表。
    /// </summary>
    /// <param name="elements">元素列表。</param>
    /// <returns>ElementId 列表。</returns>
    [Pure]
    public static List<ElementId> ToElementIds(this IEnumerable<Element> elements)
    {
        return elements.Select(element => element.Id).ToList();
    }

    /// <param name="id"></param>
    extension(ElementId id)
    {
        /// <summary>
        /// 将 ElementId 转换为 Element。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="document"></param>
        /// <returns></returns>
        [Pure]
        public T ToElement<T>(Document document)
            where T : Element
        {
            return (T)document.GetElement(id);
        }

        /// <summary>
        /// 将 ElementId 转换为 Element。
        /// </summary>
        /// <param name="doc">文档。</param>
        /// <returns>Element 对象。</returns>
        [Pure]
        public Element ToElement(Document doc) => doc.GetElement(id);
    }

    /// <summary>
    /// 获取ID值
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
#if REVIT2024_OR_GREATER
    [Pure]
    public static long GetValue(this ElementId id)
    {
        return id.Value;
    }
#else
    [Pure]
    public static int GetValue(this ElementId id)
    {
        return id.IntegerValue;
    }
#endif
}