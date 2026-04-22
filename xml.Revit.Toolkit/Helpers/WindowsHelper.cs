/* 作    者: zedmoster
** 创建时间: 2024/11/16 16:16:12
**
** Copyright 2024 by zedmoster
** Permission to use, copy, modify, and distribute this software in
** object code form for any purpose and without fee is hereby granted,
** provided that the above copyright notice appears in all copies and
** that both that copyright notice and the limited warranty and
** restricted rights notice below appear in all supporting
** documentation.
*/

using System.Windows;
using Microsoft.Win32;
using xml.Revit.Toolkit.Utils;

namespace xml.Revit.Toolkit.Helpers
{
    internal sealed class WindowsHelper
    {
        private const string key = "RestoreBounds";

        private const string RegistryKeyPath = "Software\\xml.Revit.Windows";

        public static void SetSize(Window window)
        {
            try
            {
                if (window.SizeToContent != SizeToContent.Manual)
                {
                    return;
                }

                var registryPath = System.IO.Path.Combine(
                    RegistryKeyPath,
                    window.GetType().FullName
                );
                using RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(registryPath);
                if (registryKey != null)
                {
                    if (registryKey.GetValue(key) is string bounds)
                    {
                        Rect windowBounds = Rect.Parse(bounds);
                        window.Top = windowBounds.Top;
                        window.Left = windowBounds.Left;
                        window.Width = windowBounds.Width;
                        window.Height = windowBounds.Height;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
            }
        }

        public static void SaveSize(Window window)
        {
            try
            {
                var registryPath = System.IO.Path.Combine(
                    RegistryKeyPath,
                    window.GetType().FullName
                );
                using RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(registryPath);
                registryKey.SetValue(key, window.RestoreBounds.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
            }
        }
    }
}
