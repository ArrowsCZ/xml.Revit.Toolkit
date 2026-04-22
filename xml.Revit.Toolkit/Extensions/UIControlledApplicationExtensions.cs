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

namespace xml.Revit.Toolkit.Extensions
{
    /// <summary>
    /// UIControlledApplication Extensions
    /// </summary>
    public static class UIControlledApplicationExtensions
    {
        /// <summary>
        /// 在附件模块创建面板
        /// </summary>
        /// <param name="application">UI控制应用程序</param>
        /// <param name="panelName">面板名称</param>
        /// <returns>创建的Ribbon面板</returns>
        public static RibbonPanel CreatePanel(
            this UIControlledApplication application,
            string panelName
        )
        {
            panelName = panelName ?? "分享功能";

            return application
                    .GetRibbonPanels(Tab.AddIns)
                    .FirstOrDefault(panel => panel.Name.Equals(panelName))
                ?? application.CreateRibbonPanel(panelName);
        }

        /// <summary>
        /// 在指定 Tab 创建面板
        /// </summary>
        /// <param name="application">UIControlledApplication 实例</param>
        /// <param name="tabName">Tab 的名称</param>
        /// <param name="panelName">要创建的面板的名称</param>
        /// <returns>创建的 RibbonPanel 实例</returns>
        public static RibbonPanel CreatePanel(
            this UIControlledApplication application,
            string tabName,
            string panelName
        )
        {
            panelName = panelName ?? "Revit二次开发教程";
            Autodesk.Windows.RibbonTab ribbonTab =
                Autodesk.Windows.ComponentManager.Ribbon.Tabs.FirstOrDefault(tab =>
                    tab.Id.Equals(tabName)
                );

            if (ribbonTab == null)
            {
                application.CreateRibbonTab(tabName);
                return application.CreateRibbonPanel(tabName, panelName);
            }

            return application
                    .GetRibbonPanels(tabName)
                    .FirstOrDefault(panel => panel.Name.Equals(panelName))
                ?? application.CreateRibbonPanel(tabName, panelName);
        }

        /// <summary>
        /// Get <see cref="Autodesk.Revit.UI.UIApplication"/> using the <paramref name="application"/>
        /// </summary>
        /// <param name="application">Revit UIApplication</param>
        public static UIApplication GetUIApplication(this UIControlledApplication application)
        {
            var type = typeof(UIControlledApplication);

            var propertie = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                .FirstOrDefault(e => e.FieldType == typeof(UIApplication));

            return propertie?.GetValue(application) as UIApplication;
        }
    }
}
