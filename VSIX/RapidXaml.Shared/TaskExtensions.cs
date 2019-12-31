// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;

namespace RapidXamlToolkit
{
    internal static class TaskExtensions
    {
        internal static void LogAndForget(this System.Threading.Tasks.Task task, string source) =>
            task.ContinueWith(
                (t, s) => VsShellUtilities.LogError(s as string, t.Exception?.ToString()),
                source,
                CancellationToken.None,
                TaskContinuationOptions.OnlyOnFaulted,
                VsTaskLibraryHelper.GetTaskScheduler(VsTaskRunContext.UIThreadNormalPriority));
    }
}
