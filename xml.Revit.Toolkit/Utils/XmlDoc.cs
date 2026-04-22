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

using Autodesk.Internal.InfoCenter;
using Autodesk.Windows;
using MessageBox = System.Windows.MessageBox;

namespace xml.Revit.Toolkit.Utils;

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
    public static UIControlledApplication CreateUiControlledApplication()
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
    public static IRevitTask RevitTaskService => XmlExternalApplication.RevitTaskService;

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
                    UIapp.Idling += UiAppIdling;
                    UIapp.Idling -= UiAppIdling;
                    return true;
                }
            }
            catch
            {
                // ignored
            }

            return false;
        }
    }

    private static void UiAppIdling(object sender, IdlingEventArgs e) { }

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
        return MessageBox.Show(msg, "提示", button, image);
    }

    /// <summary>
    /// 异常
    /// </summary>
    /// <param name="ex"></param>
    /// <returns></returns>
    public static MessageBoxResult Print(Exception ex)
    {
        Console.WriteLine(ex);
        return MessageBox.Show(ex.ToString(), "提示", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    /// <summary>
    /// Revit气泡提示
    /// </summary>
    /// <param name="title"></param>
    /// <param name="category"></param>
    /// <param name="uriString"></param>
    public static void ShowBalloon(string title, string category = null, string uriString = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            return;

        var ri = new ResultItem { Category = category ?? "提示", Title = title.Trim() };
        if (Uri.TryCreate(uriString ?? XmlHelpUrlAttribute.BaseHelpUrl, UriKind.RelativeOrAbsolute, out Uri uri))
            ri.Uri = uri;

        Console.WriteLine(category);
        Console.WriteLine(title);
        Console.WriteLine(uriString);

        ComponentManager.InfoCenterPaletteManager.ShowBalloon(ri);
    }
}
