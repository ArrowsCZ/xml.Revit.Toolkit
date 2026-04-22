/* 作    者: xml
** 创建时间: 2024/2/24 9:48:13
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
    /// UIApplication Extensions
    /// </summary>
    public static partial class UIApplicationExtensions
    {
        /// <summary>
        /// 是否已经打开了项目文档
        /// </summary>
        /// <param name="uiapp"></param>
        /// <returns></returns>
        public static bool IsOpenDocument(this UIApplication uiapp)
        {
            return uiapp.ActiveUIDocument?.Document != null;
        }
    }
}
