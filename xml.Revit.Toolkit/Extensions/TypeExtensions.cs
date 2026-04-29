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

using xml.Revit.Toolkit.Models;

namespace xml.Revit.Toolkit.Extensions;

/// <summary>
/// Type Extensions
/// </summary>
public static class TypeExtensions
{
    /// <param name="type"></param>
    extension(Type type)
    {
        /// <summary>
        /// 获取 PushButtonData
        /// </summary>
        /// <returns></returns>
        public PushButtonData GetPushButtonData() => new XmlPushButtonData(type).CreatePushButtonData();

        /// <summary>
        /// 类型是否包含指定特性标签
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool HasAttribute<T>()
            where T : Attribute
        {
#if NETCOREAPP
            var fullname = typeof(T).FullName;
            return type.GetCustomAttributes().Any(o => fullname.Equals(o.ToString()));
#else
            return type.GetCustomAttribute<T>() != null;
#endif
        }
    }
}
