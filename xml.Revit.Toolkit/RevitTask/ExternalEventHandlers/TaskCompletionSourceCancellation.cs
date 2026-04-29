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
/// <see cref="TaskCompletionSource{TResult}"/> with cancellation token.
/// </summary>
/// <typeparam name="TResult"></typeparam>
internal class TaskCompletionSourceCancellation<TResult>
    : TaskCompletionSource<TResult>,
        IDisposable
{
    private readonly IDisposable _registration;

    /// <summary>
    /// Creates a <see cref="TaskCompletionSource{TResult}"/> with cancellation token.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    public TaskCompletionSourceCancellation(CancellationToken cancellationToken = default)
    {
        if (cancellationToken == CancellationToken.None)
            cancellationToken = CancellationToken.None;

        if (cancellationToken.IsCancellationRequested)
        {
            TrySetCanceled(cancellationToken);
            return;
        }
        _registration = cancellationToken.Register(() => TrySetCanceled(cancellationToken));
    }

    /// <summary>
    /// Dispose the registration cancelAction token.
    /// </summary>
    public void Dispose()
    {
        _registration?.Dispose();
    }
}