/* 作    者: zedmoster
** 创建时间: 2024/11/17 19:50:48
**
** Copyright 2024 by zedmoster
** Permission to use, copy, modify, and distribute this software in
** object code form for any purpose and without fee is hereby granted,
** provided that the above copyright notice appears in all copies and
** that both that copyright notice and the limited warranty and
** restricted rights notice below appear in all supporting
** documentation.
*/

using System.Xml.Linq;
using xml.Revit.Toolkit.Utils;

namespace xml.Revit.Toolkit.Helpers
{
    /// <summary>
    /// 选择文件 工具类
    /// </summary>
    public static class FileHelper
    {
        /// <summary>
        /// 桌面文件夹地址
        /// </summary>
        public static readonly string deskTop = Environment.GetFolderPath(
            Environment.SpecialFolder.Desktop
        );

        /// <summary>
        /// 选择文件夹
        /// </summary>
        /// <param name="status"></param>
        /// <param name="initDir"></param>
        /// <returns></returns>
        public static string SelectFolder(string status = "请选择文件夹", string initDir = null)
        {
            FolderBrowserDialog folderBrowserDialog =
                new()
                {
                    Description = status,
                    RootFolder = Environment.SpecialFolder.Desktop,
                    SelectedPath = initDir,
                    ShowNewFolderButton = true,
                };
            folderBrowserDialog.ShowDialog();
            return folderBrowserDialog.SelectedPath;
        }

        /// <summary>
        /// 选择单个文件
        /// </summary>
        /// <param name="tpFilter"></param>
        /// <param name="title"></param>
        /// <param name="initDir"></param>
        /// <returns></returns>
        public static string SelectFile(
            string tpFilter = "Revit文件 (*.rvt)|*.rvt",
            string title = "请选择文件",
            string initDir = null
        )
        {
            OpenFileDialog openFileDialog =
                new()
                {
                    Title = title,
                    Filter = tpFilter,
                    InitialDirectory = initDir,
                    Multiselect = false,
                };
            openFileDialog.ShowDialog();
            return openFileDialog.FileName;
        }

        /// <summary>
        /// 选择多个文件
        /// </summary>
        /// <param name="tpFilter"></param>
        /// <param name="initDir"></param>
        /// <returns></returns>
        public static string[] SelectFiles(
            string tpFilter = "Revit样板文件 (*.rte)|*.rte",
            string initDir = null
        )
        {
            OpenFileDialog openFileDialog =
                new()
                {
                    Title = "请多选文件",
                    Filter = tpFilter,
                    InitialDirectory = initDir,
                    Multiselect = true,
                };
            openFileDialog.ShowDialog();
            return openFileDialog.FileNames;
        }

        /// <summary>
        /// 更新项目版本号
        /// </summary>
        /// <param name="fileName"></param>
        public static void UpdateVersion(string fileName)
        {
            try
            {
                XDocument doc = XDocument.Load(fileName);
                var assemblyVersionElement = doc.Root.Element("PropertyGroup")
                    .Element("AssemblyVersion");
                if (assemblyVersionElement != null)
                {
                    string currentVersion = assemblyVersionElement.Value;
                    string[] versionParts = currentVersion.Split('.');
                    int[] versionNumbers = Array.ConvertAll(versionParts, int.Parse);

                    // 递增最低位（补丁号）并处理进位
                    versionNumbers[versionNumbers.Length - 1]++;

                    // 从补丁号开始，检查是否需要进位
                    for (int i = versionNumbers.Length - 1; i >= 0; i--)
                    {
                        if (versionNumbers[i] == 10)
                        {
                            versionNumbers[i] = 0; // 重置当前位
                            if (i - 1 >= 0)
                            {
                                versionNumbers[i - 1]++; // 进位到上一位
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                    string newVersion = string.Join(".", versionNumbers);
                    assemblyVersionElement.Value = newVersion;
                    Console.WriteLine($"版本号已更新为: {newVersion}");
                }
                else
                {
                    Console.WriteLine("未找到 AssemblyVersion 属性。");
                }
                doc.Save(fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"更新版本号时出错: {ex.Message}", ex);
            }
        }
    }
}
