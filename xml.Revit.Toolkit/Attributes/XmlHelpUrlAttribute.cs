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
    /// 添加此特性的功能类自动设置F1跳转到设置的url地址
    /// </summary>
    /// <remarks>
    /// 示例
    /// <br>[<see cref="XmlHelpUrlAttribute"/>("https://www.zedmoster.cn")]</br>
    /// <br>[<see cref="TransactionAttribute"/>(<see cref="TransactionMode.Manual"/>)]</br>
    /// <br>public class Cmd : <see cref="Utils.XmlExternalCommand"/></br>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public sealed class XmlHelpUrlAttribute : Attribute
    {
        /// <summary>
        /// 文章url
        /// </summary>
        public const string baseHelpUrl = "https://www.zedmoster.cn/";

        /// <summary>
        /// 功能推荐页
        /// </summary>
        public string HelpUrl { get; set; } = baseHelpUrl;

        /// <summary>
        /// 验证网址
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static bool IsValidUrl(string url)
        {
            string pattern = @"^(https?|ftp)://[^\s/$.?#].[^\s]*$";
            return System.Text.RegularExpressions.Regex.IsMatch(url, pattern);
        }

        /// <summary>
        /// 默认构造器
        /// </summary>
        public XmlHelpUrlAttribute() { }

        /// <summary>
        /// 构造帮助URL
        /// </summary>
        /// <param name="helpUrl"></param>
        public XmlHelpUrlAttribute(string helpUrl)
        {
            if (IsValidUrl(helpUrl))
            {
                HelpUrl = helpUrl;
            }
        }
    }
}
