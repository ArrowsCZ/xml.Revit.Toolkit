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

using xml.Revit.Toolkit.RevitTask;

namespace xml.Revit.Toolkit.Extensions
{
    /// <summary>
    /// Extension for <see cref="IRevitTask"/>
    /// </summary>
    public static class IRevitTaskExtensions
    {
        /// <summary>
        /// Run code in Revit context.
        /// </summary>
        public static Task<TResult> RunAsync<TResult>(
            this IRevitTask revitTask,
            Func<UIApplication, TResult> function
        )
        {
            return RunAsync(revitTask, function, CancellationToken.None);
        }

        /// <summary>
        /// Run code in Revit context.
        /// </summary>
        public static Task<TResult> RunAsync<TResult>(
            this IRevitTask revitTask,
            Func<TResult> function
        )
        {
            return revitTask.RunAsync(function, CancellationToken.None);
        }

        /// <summary>
        /// Run code in Revit context.
        /// </summary>
        public static Task RunAsync(this IRevitTask revitTask, Action<UIApplication> action)
        {
            return revitTask.RunAsync(action, CancellationToken.None);
        }

        /// <summary>
        /// Run code in Revit context.
        /// </summary>
        public static Task RunAsync(this IRevitTask revitTask, Action action)
        {
            return revitTask.RunAsync(action, CancellationToken.None);
        }

        /// <summary>
        /// Run code in Revit context.
        /// </summary>
        public static Task<TResult> RunAsync<TResult>(
            this IRevitTask revitTask,
            Func<UIApplication, TResult> function,
            CancellationToken cancellationToken
        )
        {
            return revitTask.RunAsync(uiapp => function(uiapp), cancellationToken);
        }

        /// <summary>
        /// Run code in Revit context.
        /// </summary>
        public static Task<TResult> RunAsync<TResult>(
            this IRevitTask revitTask,
            Func<TResult> function,
            CancellationToken cancellationToken
        )
        {
            return revitTask.RunAsync(uiapp => function(), cancellationToken);
        }

        /// <summary>
        /// Run code in Revit context.
        /// </summary>
        public static Task RunAsync(
            this IRevitTask revitTask,
            Action<UIApplication> action,
            CancellationToken cancellationToken
        )
        {
            return revitTask.RunAsync<object>(
                uiapp =>
                {
                    action(uiapp);
                    return null;
                },
                cancellationToken
            );
        }

        /// <summary>
        /// Run code in Revit context.
        /// </summary>
        public static Task RunAsync(
            this IRevitTask revitTask,
            Action action,
            CancellationToken cancellationToken
        )
        {
            return revitTask.RunAsync(uiapp => action(), cancellationToken);
        }
    }
}
