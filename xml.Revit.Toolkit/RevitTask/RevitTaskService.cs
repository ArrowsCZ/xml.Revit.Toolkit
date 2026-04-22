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

using xml.Revit.Toolkit.Utils;

namespace xml.Revit.Toolkit.RevitTask
{
    /// <summary>
    /// RevitTask service to manage and run code in Revit context.
    /// </summary>
    /// <remarks>
    /// RevitTaskService
    /// </remarks>
    /// <param name="application"></param>
    public class RevitTaskService(UIApplication application) : IDisposable, IRevitTask
    {
        private readonly UIApplication uiapp = application;
        private readonly List<ExternalEvent> ExternalEvents = [];
        private readonly List<IExternalEventHandler> ExternalEventHandlers = [];

        /// <summary>
        /// 是否已初始化
        /// </summary>
        public bool HasInitialized { get; private set; } = false;

        /// <summary>
        /// Initialize the service
        /// </summary>
        /// <remarks>Subscribe to the <see cref="UIControlledApplication.Idling"/> event.</remarks>
        public void Initialize()
        {
            if (HasInitialized)
                return;
            uiapp.Idling += Application_Idling;
            HasInitialized = true;
        }

        /// <summary>
        /// Run code in Revit context.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="function"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<TResult> RunAsync<TResult>(
            Func<UIApplication, TResult> function,
            CancellationToken cancellationToken
        )
        {
            if (!HasInitialized)
                return Task.FromException<TResult>(
                    new Exception($"{GetType().Name} is not initialized.")
                );

            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled<TResult>(cancellationToken);

            if (XmlDoc.IsRevitInApiMode)
            {
                try
                {
                    var result = function.Invoke(uiapp);
                    return Task.FromResult(result);
                }
                catch (Exception ex)
                {
                    return Task.FromException<TResult>(ex);
                }
            }

            var asyncExternalEventHandler =
                new ExternalEventHandlers.AsyncExternalEventHandlerCancellation<TResult>(
                    function,
                    cancellationToken
                );
            ExternalEventHandlers.Add(asyncExternalEventHandler);
            return asyncExternalEventHandler.AsyncResult();
        }

        private void Application_Idling(object sender, Autodesk.Revit.UI.Events.IdlingEventArgs e)
        {
            foreach (var asyncExternalEventHandler in ExternalEventHandlers)
            {
                if (ExternalEventHandlers.Remove(asyncExternalEventHandler))
                {
                    if (asyncExternalEventHandler is IDisposable disposable)
                        disposable.Dispose();

                    var externalEvent = ExternalEvent.Create(asyncExternalEventHandler);
                    externalEvent.Raise();
                    ExternalEvents.Add(externalEvent);
                    break;
                }
            }

            foreach (var externalEvent in ExternalEvents)
            {
                if (externalEvent.IsPending)
                    continue;

                if (ExternalEvents.Remove(externalEvent))
                {
                    externalEvent.Dispose();
                    break;
                }
            }
        }

        /// <summary>
        /// Dispose the service
        /// </summary>
        /// <remarks>Unsubscribe to the <see cref="UIControlledApplication.Idling"/> event.</remarks>
        public void Dispose()
        {
            if (!HasInitialized)
                return;
            uiapp.Idling -= Application_Idling;

            ExternalEvents.Clear();
            ExternalEventHandlers.Clear();
            HasInitialized = false;
        }
    }
}
