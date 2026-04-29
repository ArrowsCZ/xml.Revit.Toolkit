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

using System.Diagnostics;

namespace xml.Revit.Toolkit.Extensions;

/// <summary>
/// Transaction Extensions
/// </summary>
public static class TransactionExtensions
{
    /// <summary>
    /// 内置名称
    /// </summary>
    private const string TransactionName = "zedmoster";

    /// <param name="doc">文档</param>
    extension(Document doc)
    {
        /// <summary>
        /// 封装事务组方法
        /// </summary>
        /// <param name="action">要执行的操作</param>
        /// <param name="name">事务组名称</param>
        public void TransactionGroup(Action<TransactionGroup> action, string name = null)
        {
            name = string.IsNullOrWhiteSpace(name) ? TransactionName : name;
            using TransactionGroup tg = new(doc, name);
            tg.Start();
            action(tg);
            if (tg.HasStarted() && tg.GetStatus() == TransactionStatus.Started)
                tg.Assimilate();
            else
                tg.RollBack();
        }

        /// <summary>
        /// 封装事务方法
        /// </summary>
        /// <param name="action">要执行的操作</param>
        /// <param name="name">事务名称</param>
        /// <param name="ignoreFailure">是否忽略失败</param>
        public void Transaction(Action<Transaction> action, string name = null, bool ignoreFailure = true)
        {
            name = string.IsNullOrWhiteSpace(name) ? TransactionName : name;

            using Transaction t = new(doc, name);
            try
            {
                t.Start();
                if (ignoreFailure)
                    IgnoreFailure(t);
                action(t);
                if (t.HasStarted() && t.GetStatus() == TransactionStatus.Started)
                {
                    t.Commit();
                }
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine("Canceled");
                t.RollBack();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                t.RollBack();
                throw;
            }
        }

        /// <summary>
        /// 子事务
        /// </summary>
        /// <param name="action"></param>
        public void SubTransaction(Action<SubTransaction> action)
        {
            using SubTransaction t = new(doc);
            try
            {
                t.Start();
                action(t);
                if (t.HasStarted() && t.GetStatus() == TransactionStatus.Started)
                {
                    t.Commit();
                }
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine("Canceled");
                t.RollBack();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                t.RollBack();
                throw;
            }
        }
    }

    /// <summary>
    /// 忽略警告弹窗提示
    /// </summary>
    /// <param name="t"> 事务</param>
    private static void IgnoreFailure(Transaction t)
    {
        var fho = t.GetFailureHandlingOptions();
        fho.SetFailuresPreprocessor(new FailuresPreprocessorBase());
        t.SetFailureHandlingOptions(fho);
    }
}

/// <summary>
/// IFailuresPreprocessor
/// </summary>
public class FailuresPreprocessorBase : IFailuresPreprocessor
{
    /// <summary>
    /// 忽略弹窗错误警告
    /// </summary>
    public static bool IsIgnoreError { get; set; } = false;

    /// <summary>
    /// IFailuresPreprocessor.PreprocessFailures
    /// </summary>
    /// <param name="failuresAccessor"></param>
    /// <returns></returns>
    public virtual FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
    {
        var failures = failuresAccessor.GetFailureMessages();
        if (failures.Count > 0)
        {
            foreach (FailureMessageAccessor f in failures)
            {
                switch (f.GetSeverity())
                {
                    case FailureSeverity.None:
                        break;
                    case FailureSeverity.Warning:
                        if (f.HasResolutions())
                            failuresAccessor.ResolveFailure(f);
                        else
                            failuresAccessor.DeleteWarning(f);

                        return FailureProcessingResult.ProceedWithCommit;
                    case FailureSeverity.Error:
                        if (IsIgnoreError)
                        {
                            failuresAccessor.DeleteAllWarnings();
                            return FailureProcessingResult.ProceedWithCommit;
                        }

                        return FailureProcessingResult.WaitForUserInput;
                    case FailureSeverity.DocumentCorruption:
                        return FailureProcessingResult.ProceedWithRollBack;
                }
            }
        }
        return FailureProcessingResult.Continue;
    }
}
