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

namespace xml.Revit.Toolkit.Extensions
{
    /// <summary>
    /// Reference Extensions
    /// </summary>
    public static class ReferenceExtensions
    {
        /// <summary>
        /// 将 Reference 转换为 Element。如果 Element 是 RevitLinkInstance，则获取链接文档中的 LinkedElement。
        /// </summary>
        /// <param name="reference">要转换的 Reference。</param>
        /// <param name="doc">关联的文档。</param>
        /// <returns>转换后的 Element。</returns>
        public static Element ToElement(this Reference reference, Document doc)
        {
            if (reference is null)
            {
                return null;
            }
            var element = doc.GetElement(reference);
            if (element != null && element is RevitLinkInstance linkInstance)
            {
                Document linkDocument = linkInstance.GetLinkDocument();
                Element linkedElement = linkDocument.GetElement(reference.LinkedElementId);
                return linkedElement;
            }

            return element;
        }

        /// <summary>
        /// 将 Reference 集合转换为指定类型的 Element 列表。
        /// </summary>
        /// <typeparam name="T">要转换的元素类型。</typeparam>
        /// <param name="references">要转换的 Reference 集合。</param>
        /// <param name="doc">关联的文档。</param>
        /// <returns>转换后的 Element 列表。</returns>
        public static List<T> ToElements<T>(this IEnumerable<Reference> references, Document doc)
            where T : Element
        {
            return references
                .Select(reference => reference.ToElement(doc) as T)
                .Where(element => element != null)
                .ToList();
        }

        /// <summary>
        /// Reference 转 链接文档 实例
        /// </summary>
        /// <param name="value"> reference</param>
        /// <param name="doc"> document</param>
        /// <returns>not linkdoc result null</returns>
        public static RevitLinkInstance ToLinkInstance(this Reference value, Document doc)
        {
            return doc.GetElement(value) as RevitLinkInstance;
        }
    }
}
