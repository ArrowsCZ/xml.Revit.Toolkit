/* 作    者: xml
 ** 创建时间: 2024/2/25 19:09:30
 **
 ** Copyright 2024 by zedmoster
 ** Permission to use, copy, modify, and distribute this software in
 ** object code form for any purpose and without fee is hereby granted,
 ** provided that the above copyright notice appears in all copies and
 ** that both that copyright notice and the limited warranty and
 ** restricted rights notice below appear in all supporting
 ** documentation.
 */

using Autodesk.Windows;
using Visibility = System.Windows.Visibility;

namespace xml.Revit.Toolkit.Helpers;

/// <summary>
/// XmlWindow Collector
/// </summary>
public static class WindowCollector
{
    private static readonly List<Window> Windows = [];

    /// <summary>
    /// Makes a window visible
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static void Show<T>()
        where T : Window, new()
    {
        if (FocusNull<T>())
        {
            T window = new();
            Show(window);
        }
    }

    /// <summary>
    ///     Opens a window and returns without waiting for the newly opened window to close
    /// </summary>
    public static void Show(Window window)
    {
        Attach(window);
        new WindowInteropHelper(window).Owner = ComponentManager
            .ApplicationWindow;
        window.Show();
    }

    /// <summary>
    ///     Makes a window invisible
    /// </summary>
    /// <typeparam name="T">Type of <see cref="T:System.Windows.Window" /></typeparam>
    public static void Hide<T>()
        where T : Window
    {
        var type = typeof(T);
        foreach (var window in Windows)
            if (window.GetType() == type)
                window.Hide();
    }

    /// <summary>
    ///     Manually closes a <see cref="T:System.Windows.Window" />
    /// </summary>
    /// <typeparam name="T">Type of window</typeparam>
    public static void Close<T>()
        where T : Window
    {
        var type = typeof(T);
        for (var i = Windows.Count - 1; i >= 0; i--)
        {
            var window = Windows[i];
            if (window.GetType() == type)
                window.Close();
        }
    }

    /// <summary>
    ///     Attempts to set focus to this element
    /// </summary>
    /// <typeparam name="T">Type of <see cref="T:System.Windows.Window" /></typeparam>
    /// <returns>Flase if the window instance has already been created</returns>
    public static bool FocusNull<T>()
        where T : Window
    {
        var type = typeof(T);
        foreach (var window in Windows)
            if (window.GetType() == type)
            {
                if (window.WindowState == WindowState.Minimized)
                    window.WindowState = WindowState.Normal;
                if (window.Visibility != Visibility.Visible)
                    window.Show();
                window.Focus();
                return false;
            }

        return true;
    }

    private static void Attach(Window window)
    {
        Windows.Add(window);
        window.Closed += (sender, _) =>
        {
            var modelessWindow = (Window)sender;
            Windows.Remove(modelessWindow);
        };
    }
}