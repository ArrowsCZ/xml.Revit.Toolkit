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

using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;
using xml.Revit.Toolkit.Attributes;
using xml.Revit.Toolkit.Extensions;

namespace xml.Revit.Toolkit.Models
{
    /// <summary>
    /// 功能类
    /// </summary>
    public sealed partial class XmlPushButtonData
    {
        /// <summary>
        /// 路径
        /// </summary>
        public static string DllPath { get; private set; }

        /// <summary>
        /// 类型
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// 修改 面板
        /// </summary>
        public bool Modify { get; private set; }

        /// <summary>
        /// 内部名称
        /// </summary>
        public string InName => $"xml_{ButtonName}";

        /// <summary>
        /// 功能名称
        /// </summary>
        public string ButtonName { get; private set; }

        /// <summary>
        /// 功能命名空间
        /// </summary>
        public string NameSpace { get; private set; }

        /// <summary>
        /// 程序集名称
        /// </summary>
        public string AssemblyName { get; private set; }

        /// <summary>
        /// 功能提示
        /// </summary>
        public string Tooltip { get; private set; }

        /// <summary>
        /// 功能长按提示
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// 功能icon
        /// <para>Icons读取插件目录文件夹</para>
        /// </summary>
        public BitmapSource Image { get; set; }

        /// <summary>
        /// 功能小图标
        /// <para>Icons读取插件目录文件夹</para>
        /// </summary>
        public BitmapSource StackedImage { get; set; }

        /// <summary>
        /// 功能提示图片
        /// <para>Icons读取插件目录文件夹</para>
        /// </summary>
        public BitmapSource ToolImage { get; set; }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="type"></param>
        public XmlPushButtonData(Type type)
        {
            Type = type;
            DllPath = Path.GetDirectoryName(Type.Assembly.Location);
            if (Type != null)
            {
                AssemblyName = Type.Assembly.Location;
                NameSpace = Type.FullName;
#if NETCOREAPP
                Modify = Type.HasAttribute<XmlModifyAttribute>();
                var xmlAttribute = Type.GetCustomAttributesData()
                    .FirstOrDefault(o =>
                        o.AttributeType.ToString().Equals(typeof(XmlAttribute).FullName)
                    );
                if (xmlAttribute == null)
                {
                    ButtonName = Type.Name;
                    Tooltip = Type.Name;
                    Description = "";
                }
                else
                {
                    ButtonName =
                        xmlAttribute.ConstructorArguments.ElementAtOrDefault(0).Value?.ToString()
                        ?? Type.Name;
                    Tooltip =
                        xmlAttribute.ConstructorArguments.ElementAtOrDefault(1).Value?.ToString()
                        ?? Type.Name;
                    Description =
                        xmlAttribute.ConstructorArguments.ElementAtOrDefault(2).Value?.ToString()
                        ?? "";
                }
#else
                var xmlAttribute = Type.GetCustomAttribute<XmlAttribute>();
                Modify = Type.HasAttribute<XmlModifyAttribute>();
                ButtonName = xmlAttribute?.Name ?? Type.Name;
                Tooltip = xmlAttribute?.ToolTip ?? Type.Name;
                Description = xmlAttribute?.Description ?? "";
#endif
                var name = ButtonName.Replace("\n", "");
                var bitmap = GetLoaclResourcesByImageName(name);
                Image = bitmap;
                StackedImage = bitmap.ResizeImage();
            }
        }

        /// <summary>
        /// 读取当前程序集中的图标资源
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static BitmapSource GetResourcesByImageName(Assembly assembly, string fileName)
        {
            // 获取该程序集内所有的资源名称
            string[] resourceNames = assembly.GetManifestResourceNames();

            // 查找与指定文件名匹配的资源
            string resourceName = resourceNames.FirstOrDefault(o =>
                o.EndsWith($"{fileName}.png", StringComparison.OrdinalIgnoreCase)
            );

            if (resourceName != null)
            {
                // 如果找到了匹配的资源，读取该资源并返回图标
                using (Stream resourceStream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (resourceStream != null)
                    {
                        BitmapImage bitmapImage = new();
                        bitmapImage.BeginInit();
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.DecodePixelWidth = 32;
                        bitmapImage.StreamSource = resourceStream;
                        bitmapImage.EndInit();
                        bitmapImage.Freeze();
                        return bitmapImage;
                    }
                }
            }
            return default;
        }

        /// <summary>
        /// 尝试从当前程序集及其所有引用的程序集的嵌入资源中加载图标
        /// </summary>
        /// <param name="fileName">图标文件名（不包括扩展名）</param>
        /// <returns></returns>
        private static BitmapSource GetEmbeddedResourceImage(string fileName)
        {
            // 获取当前应用程序域中的所有已加载程序集
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            // 遍历每个程序集
            foreach (var assembly in assemblies)
            {
                try
                {
                    var bitmap = GetResourcesByImageName(assembly, fileName);
                    if (bitmap != null)
                    {
                        return bitmap;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            // 如果没有找到图标，则返回默认
            return GetBitmapImageDefault();
        }

        /// <summary>
        /// 获取默认图标资源
        /// </summary>
        /// <returns></returns>
        public static BitmapImage GetBitmapImageDefault()
        {
            var uri = new Uri(
                "pack://application:,,,/xml.Revit.Toolkit;component/Resources/icon.png",
                UriKind.Absolute
            );
            return new BitmapImage(uri);
        }

        /// <summary>
        /// 获取当前程序集所在目录下 Resources 文件夹内指定名称的 png 图标
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static BitmapSource GetLoaclResourcesByImageName(string fileName)
        {
            try
            {
                var bitmap = GetEmbeddedResourceImage(fileName);
                if (bitmap != null)
                {
                    return bitmap;
                }

                List<string> paths =
                [
                    Path.Combine(DllPath, "Resources", $"{fileName}.png"),
                    Path.Combine(DllPath, "Icons", $"{fileName}.png"),
                    Path.Combine(DllPath, "图标", $"{fileName}.png"),
                ];
                foreach (string imagePath in paths)
                {
                    if (File.Exists(imagePath))
                    {
                        BitmapImage bitmapImage = new();
                        using (
                            FileStream fileStream = new(imagePath, FileMode.Open, FileAccess.Read)
                        )
                        {
                            bitmapImage.BeginInit();
                            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                            bitmapImage.DecodePixelWidth = 32;
                            bitmapImage.StreamSource = fileStream;
                            bitmapImage.EndInit();
                            bitmapImage.Freeze();
                        }
                        return bitmapImage;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return null;
        }

        /// <summary>
        /// 创建 <see cref="PushButtonData"/> 功能数据
        /// </summary>
        /// <returns></returns>
        public PushButtonData CreatePushButtonData()
        {
            var buttonData = new PushButtonData(InName, ButtonName, AssemblyName, NameSpace)
            {
                ToolTip = Tooltip,
                LongDescription = Description,
                LargeImage = Image,
                Image = StackedImage,
            };

            if (typeof(IExternalCommandAvailability).IsAssignableFrom(Type))
            {
                buttonData.AvailabilityClassName = Type.FullName;
            }

            var xmlHelpUrlAttribute = Type.GetCustomAttribute<XmlHelpUrlAttribute>();
            if (xmlHelpUrlAttribute != null)
            {
                buttonData.SetContextualHelp(
                    new ContextualHelp(ContextualHelpType.Url, xmlHelpUrlAttribute.HelpUrl)
                );
            }

            return buttonData;
        }
    }
}
