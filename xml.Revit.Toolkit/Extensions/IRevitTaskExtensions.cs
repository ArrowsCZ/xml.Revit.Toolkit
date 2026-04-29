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

namespace xml.Revit.Toolkit.Extensions;

/// <summary>
/// Extension for <see cref="IRevitTask"/>
/// </summary>
public static class IRevitTaskExtensions
{
    extension(IRevitTask revitTask)
    {
        /// <summary>
        /// Run code in Revit context.
        /// </summary>
        public Task<TResult> RunAsync<TResult>(Func<UIApplication, TResult> function) =>
            RunAsync(revitTask, function, CancellationToken.None);

        /// <summary>
        /// Run code in Revit context.
        /// </summary>
        public Task<TResult> RunAsync<TResult>(Func<TResult> function) =>
            revitTask.RunAsync(function, CancellationToken.None);

        /// <summary>
        /// Run code in Revit context.
        /// </summary>
        public Task RunAsync(Action<UIApplication> action) => revitTask.RunAsync(action, CancellationToken.None);

        /// <summary>
        /// Run code in Revit context.
        /// </summary>
        public Task RunAsync(Action action) => revitTask.RunAsync(action, CancellationToken.None);

        /// <summary>
        /// Run code in Revit context.
        /// </summary>
        public Task<TResult> RunAsync<TResult>(
            Func<UIApplication, TResult> function,
            CancellationToken cancellationToken
        ) => revitTask.RunAsync(function, cancellationToken);

        /// <summary>
        /// Run code in Revit context.
        /// </summary>
        public Task<TResult> RunAsync<TResult>(Func<TResult> function, CancellationToken cancellationToken) =>
            revitTask.RunAsync(_ => function(), cancellationToken);

        /// <summary>
        /// Run code in Revit context.
        /// </summary>
        public Task RunAsync(Action<UIApplication> action, CancellationToken cancellationToken) =>
            revitTask.RunAsync<object>(
                uiapp =>
                {
                    action(uiapp);
                    return null;
                },
                cancellationToken
            );

        /// <summary>
        /// Run code in Revit context.
        /// </summary>
        public Task RunAsync(Action action, CancellationToken cancellationToken) =>
            revitTask.RunAsync(_ => action(), cancellationToken);
    }
}
