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

namespace xml.Revit.Toolkit.RevitTask.ExternalEventHandlers
{
    /// <summary>
    /// AsyncExternalEventHandler
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <remarks>
    /// AsyncExternalEventHandler
    /// </remarks>
    /// <param name="function"></param>
    public class AsyncExternalEventHandler<TResult>(Func<UIApplication, TResult> function)
        : IExternalEventHandler
    {
        private readonly Func<UIApplication, TResult> function = function;
        private readonly TaskCompletionSource<TResult> tcs = new();

        /// <summary>
        /// AsyncResult
        /// </summary>
        /// <returns></returns>
        public Task<TResult> AsyncResult()
        {
            return tcs.Task;
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
                tcs.TrySetResult(result);
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
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
    }
}
