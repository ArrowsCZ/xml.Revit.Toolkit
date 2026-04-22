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

namespace xml.Revit.Toolkit.RevitTask
{
    /// <summary>
    /// Interface to run code in Revit Context
    /// </summary>
    public interface IRevitTask
    {
        /// <summary>
        /// Has Initialized RevitTask
        /// </summary>
        bool HasInitialized { get; }

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
        );
    }
}
