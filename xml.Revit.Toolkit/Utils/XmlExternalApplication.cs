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
using xml.Revit.Toolkit.Attributes;
using xml.Revit.Toolkit.Extensions;
using xml.Revit.Toolkit.Helpers;
using xml.Revit.Toolkit.RevitTask;

namespace xml.Revit.Toolkit.Utils
{
    /// <summary>
    /// IExternalApplication 抽象类
    /// </summary>
    public abstract class XmlExternalApplication : IExternalApplication
    {
        internal static RevitTaskService _revitTaskService;

        /// <summary>
        /// 更新当前激活的窗口标题背景颜色
        /// </summary>
        public virtual bool SetTabItemColor { get; set; } = false;

        /// <summary>
        /// IExternalApplication OnStartup
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        public Result OnStartup(UIControlledApplication application)
        {
            try
            {
                var currentType = GetType();
#if NETCOREAPP
                if (
                    System.Runtime.Loader.AssemblyLoadContext.GetLoadContext(currentType.Assembly)
                    == System.Runtime.Loader.AssemblyLoadContext.Default
                )
                {
                    ResolveHelper.BeginAssemblyResolve(currentType);
                }
#else
                ResolveHelper.BeginAssemblyResolve(currentType);
#endif
                XmlDoc.UIapp ??= application.GetUIApplication();
                ControlledApplication = application;
                _revitTaskService = new RevitTaskService(UIapp);
                _revitTaskService.Initialize();

                OnStartup();
            }
            finally
            {
                ResolveHelper.EndAssemblyResolve();
            }

            return Result.Succeeded;
        }

        /// <summary>
        /// IExternalApplication OnShutdown
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        public Result OnShutdown(UIControlledApplication application)
        {
            try
            {
                var currentType = GetType();
#if NETCOREAPP
                if (
                    System.Runtime.Loader.AssemblyLoadContext.GetLoadContext(currentType.Assembly)
                    == System.Runtime.Loader.AssemblyLoadContext.Default
                )
                {
                    ResolveHelper.BeginAssemblyResolve(currentType);
                }
#else
                ResolveHelper.BeginAssemblyResolve(currentType);
#endif
                OnShutdown();
                _revitTaskService?.Dispose();
            }
            finally
            {
                ResolveHelper.EndAssemblyResolve();
            }

            return Result.Succeeded;
        }

        /// <summary>
        /// UIControlledApplication
        /// </summary>
        protected UIControlledApplication ControlledApplication { get; private set; }

        private UIApplication _uiapp;

        /// <summary>
        /// UiApplication
        /// </summary>
        protected UIApplication UIapp
        {
            get
            {
                _uiapp ??= ControlledApplication.GetUIApplication();
                return _uiapp;
            }
        }

        /// <summary>
        /// 功能名称(空则在附加模块创建)
        ///
        /// </summary>
        public virtual string TabName { get; } = string.Empty;

        /// <summary>
        /// 面板名称(不能为空)
        /// </summary>
        public abstract string PanelName { get; }

        /// <summary>
        /// 面板
        /// </summary>
        protected RibbonPanel XmlPanel { get; set; }

        /// <summary>
        /// 启动时
        /// </summary>
        public virtual void OnStartup()
        {
            XmlPanel = string.IsNullOrEmpty(TabName)
                ? ControlledApplication.CreatePanel(PanelName)
                : ControlledApplication.CreatePanel(TabName, PanelName);

            #region 添加当前程序功能

            var types = GetType()
                .Assembly.GetTypes()
                .Where(o => typeof(IExternalCommand).IsAssignableFrom(o));
            var cmds = types
                .Where(o => o.GetCustomAttribute<XmlAttribute>() != null)
                .Where(o => o.GetCustomAttribute<XmlUnfinishedAttribute>() == null)
                .OrderBy(o => o.Name);
            foreach (var item in cmds)
            {
                XmlPanel.AddItem(item.GetPushButtonData());
            }

            #endregion
        }

        /// <summary>
        /// 关闭时
        /// </summary>
        public virtual void OnShutdown()
        {
            Console.WriteLine("关闭软件");
        }
    }
}
