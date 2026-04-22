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

using System.Reflection;
using System.Windows;
using Autodesk.Revit.UI.Events;
using xml.Revit.Toolkit.Attributes;
using xml.Revit.Toolkit.RevitTask;

namespace xml.Revit.Toolkit.Utils
{
    /// <summary>
    /// XmlDoc
    /// </summary>
    public static class XmlDoc
    {
        /// <summary>
        /// UIApplication
        /// </summary>
        public static UIApplication UIapp { get; internal set; }

        /// <summary>
        /// UIDocument
        /// </summary>
        public static UIDocument UIdoc => UIapp.ActiveUIDocument;

        /// <summary>
        /// Document
        /// </summary>
        public static Document Doc => UIapp.ActiveUIDocument?.Document;

        /// <summary>
        /// UIControlledApplication
        /// </summary>
        /// <returns></returns>
        public static UIControlledApplication CreateUIControlledApplication()
        {
            return (UIControlledApplication)
                Activator.CreateInstance(
                    typeof(UIControlledApplication),
                    BindingFlags.Instance | BindingFlags.NonPublic,
                    null,
                    [UIapp],
                    null
                );
        }

        /// <summary>
        /// IRevitTask
        /// </summary>
        public static IRevitTask RevitTaskService => XmlExternalApplication._revitTaskService;

        /// <summary>
        /// 确定 Revit 是否处于 API 模式
        /// </summary>
        public static bool IsRevitInApiMode
        {
            get
            {
                try
                {
                    if (UIapp != null)
                    {
                        UIapp.Idling += UIapp_Idling;
                        UIapp.Idling -= UIapp_Idling;
                        return true;
                    }
                }
                catch { }
                return false;
            }
        }

        private static void UIapp_Idling(object sender, IdlingEventArgs e) { }

        /// <summary>
        /// 弹窗提示
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="button"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        public static MessageBoxResult Print(
            string msg,
            MessageBoxButton button = MessageBoxButton.OK,
            MessageBoxImage image = MessageBoxImage.Information
        )
        {
            Console.WriteLine(msg);
            return System.Windows.MessageBox.Show(msg.ToString(), "提示", button, image);
        }

        /// <summary>
        /// 异常
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static MessageBoxResult Print(Exception ex)
        {
            Console.WriteLine(ex);
            return System.Windows.MessageBox.Show(
                ex.ToString(),
                "提示",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }

        /// <summary>
        /// Revit气泡提示
        /// </summary>
        /// <param name="title"></param>
        /// <param name="category"></param>
        /// <param name="uriString"></param>
        public static void ShowBalloon(
            string title,
            string category = null,
            string uriString = null
        )
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return;
            }

            var ri = new Autodesk.Internal.InfoCenter.ResultItem
            {
                Category = category ?? "提示",
                Title = title.Trim(),
            };
            if (
                Uri.TryCreate(
                    uriString ?? XmlHelpUrlAttribute.baseHelpUrl,
                    UriKind.RelativeOrAbsolute,
                    out Uri uri
                )
            )
            {
                ri.Uri = uri;
            }

            Console.WriteLine(category);
            Console.WriteLine(title);
            Console.WriteLine(uriString);

            Autodesk.Windows.ComponentManager.InfoCenterPaletteManager.ShowBalloon(ri);
        }
    }
}
