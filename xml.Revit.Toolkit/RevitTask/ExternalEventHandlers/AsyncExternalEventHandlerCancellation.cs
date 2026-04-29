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

namespace xml.Revit.Toolkit.RevitTask.ExternalEventHandlers;

/// <summary>
/// AsyncExternalEventHandlerCancellation
/// </summary>
/// <typeparam name="TResult"></typeparam>
/// <remarks>
/// AsyncExternalEventHandlerCancellation
/// </remarks>
/// <param name="function"></param>
/// <param name="cancellationToken"></param>
public class AsyncExternalEventHandlerCancellation<TResult>(
    Func<UIApplication, TResult> function,
    CancellationToken cancellationToken
) : IExternalEventHandler, IDisposable
{
    private readonly TaskCompletionSourceCancellation<TResult> _tcs = new(cancellationToken);

    /// <summary>
    /// AsyncResult
    /// </summary>
    /// <returns></returns>
    public Task<TResult> AsyncResult()
    {
        return _tcs.Task;
    }

    /// <summary>
    /// This method is called to handle the external event.
    /// </summary>
    /// <param name="uiapp"></param>
    public void Execute(UIApplication uiapp)
    {
        try
        {
            if (AsyncResult().IsCompleted)
                return;

            var result = function.Invoke(uiapp);
            _tcs.TrySetResult(result);
        }
        catch (Exception ex)
        {
            _tcs.TrySetException(ex);
        }
    }

    /// <summary>
    /// String identification of the event handler.
    /// </summary>
    public string GetName()
    {
        return GetType().Name;
    }

    /// <summary>
    /// Create <see cref="Autodesk.Revit.UI.ExternalEvent"/> using the <see cref="AsyncExternalEventHandler{TResult}"/>
    /// </summary>
    public ExternalEvent Create()
    {
        return ExternalEvent.Create(this);
    }

    /// <summary>
    /// Dispose the registration cancelation token.
    /// </summary>
    public void Dispose()
    {
        _tcs.Dispose();
    }
}