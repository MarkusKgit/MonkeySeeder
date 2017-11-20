using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MonkeySeeder.Helpers
{
    public static class Extensions
    {
        public static T Evaluate<T>(this BindingExpression bindingExpr)
        {
            Type resolvedType = bindingExpr.ResolvedSource.GetType();
            PropertyInfo prop = resolvedType.GetProperty(
                bindingExpr.ResolvedSourcePropertyName);
            return (T)prop.GetValue(bindingExpr.ResolvedSource);
        }

        public static async Task<T> WithCancellation<T>(
    this Task<T> task, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            using (cancellationToken.Register(
                        s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))
                if (task != await Task.WhenAny(task, tcs.Task))
                    throw new OperationCanceledException(cancellationToken);
            return await task;
        }
    }
}