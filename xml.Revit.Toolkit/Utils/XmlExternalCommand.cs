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

namespace xml.Revit.Toolkit.Utils;

/// <summary>
/// IExternalCommand 抽象类
/// </summary>
public abstract class XmlExternalCommand : IExternalCommand, IExternalCommandAvailability
{
    /// <summary>
    /// 当前程序集所在文件夹
    /// </summary>
    protected static string Location => XmlPushButtonData.DllPath;

    /// <summary>
    ///     Element set indicating problem elements to display in the failure dialog. This will be used only if the command status was "Failed".
    /// </summary>
    public ElementSet ElementSet { get; private set; } = null!;

    /// <summary>
    ///     Reference to the <see cref="Autodesk.Revit.UI.ExternalCommandData" /> that is needed by an external command
    /// </summary>
    public ExternalCommandData ExternalCommandData { get; private set; } = null!;

    /// <summary>
    ///     Informs Autodesk Revit of the status of your application after execution.
    /// </summary>
    /// <remarks>
    ///     The result indicates if the execution fails, succeeds, or was canceled by user. If it does not
    ///     succeed, Revit will undo any changes made by the external command
    /// </remarks>
    public Result Result { get; set; } = Result.Succeeded;

    /// <summary>
    ///     Error message can be returned by external command. This will be displayed only if the command status was "Failed" <br />
    ///     There is a limit of 1023 characters for this message; strings longer than this will be truncated.
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>
    /// uidoc
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Style",
        "IDE1006:命名样式",
        Justification = "<挂起>"
    )]
    protected static UIDocument uidoc => XmlDoc.UIdoc;

    /// <summary>
    /// 设置功能可用性
    /// </summary>
    /// <param name="a"></param>
    /// <param name="s"></param>
    /// <returns></returns>
    public virtual bool SetCommandAvailability(UIApplication a, CategorySet s)
    {
        return a.IsOpenDocument();
    }

    /// <summary>Callback invoked by Revit. Not used to be called in user code</summary>
    public bool IsCommandAvailable(
        UIApplication applicationData,
        CategorySet selectedCategories
    )
    {
        return SetCommandAvailability(applicationData, selectedCategories);
    }

    /// <summary>
    /// 功能实现
    /// </summary>
    protected abstract void Execute(ref string message, ElementSet elements);

    /// <summary>
    /// IExternalCommand
    /// </summary>
    /// <param name="commandData"></param>
    /// <param name="message"></param>
    /// <param name="elements"></param>
    /// <returns></returns>
    public Result Execute(
        ExternalCommandData commandData,
        ref string message,
        ElementSet elements
    )
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

            ElementSet = elements;
            ErrorMessage = message;
            ExternalCommandData = commandData;

            if (!currentType.HasAttribute<XmlAttribute>())
            {
                XmlDoc.Print(
                    "请将功能添加特性 xml.Revit.Toolkit.Attributes.XmlAttribute 定义功能名称"
                );
                return Result.Cancelled;
            }

            XmlExternalApplication.RevitTaskService ??= new RevitTaskService(XmlDoc.UIapp);
            if (!XmlExternalApplication.RevitTaskService.HasInitialized)
            {
                XmlExternalApplication.RevitTaskService.Initialize();
            }

            Execute(ref message, elements);
            Result = Result.Succeeded;
        }
        catch (Autodesk.Revit.Exceptions.OperationCanceledException)
        {
            Console.WriteLine("已取消");
            return Result.Cancelled;
        }
        finally
        {
            message = ErrorMessage;
            ResolveHelper.EndAssemblyResolve();
        }
        return Result;
    }
}
