using Elsa.Builders;
using Elsa.Services.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Elsa.Activities.File
{
    public static class OutFileBuilderExtensions
    {
        public static IActivityBuilder OutFile(this IBuilder builder, Action<ISetupActivity<OutFile>> setup, [CallerLineNumber] int lineNumber = default, [CallerFilePath] string? sourceFile = default) => builder.Then(setup, null, lineNumber, sourceFile);

        public static IActivityBuilder OutFile(this IBuilder builder, Func<ActivityExecutionContext, byte[]> data, Func<ActivityExecutionContext, string> path, Func<ActivityExecutionContext, CopyMode> mode, [CallerLineNumber] int lineNumber = default, [CallerFilePath] string? sourceFile = default) =>
            builder.OutFile(activity => activity
                    .Set(x => x.Data, data)
                    .Set(x => x.Path, path)
                    .Set(x => x.Mode, mode), 
                lineNumber, 
                sourceFile);

        public static IActivityBuilder OutFile(this IBuilder builder, Func<ActivityExecutionContext, ValueTask<byte[]>> data, Func<ActivityExecutionContext, ValueTask<string>> path, Func<ActivityExecutionContext, ValueTask<CopyMode>> mode, [CallerLineNumber] int lineNumber = default, [CallerFilePath] string? sourceFile = default) =>
            builder.OutFile(activity => activity
                    .Set(x => x.Data, data)
                    .Set(x => x.Path, path)
                    .Set(x => x.Mode, mode),
                lineNumber,
                sourceFile);

        public static IActivityBuilder OutFile(this IBuilder builder, Func<byte[]> data, Func<string> path, Func<CopyMode> mode, [CallerLineNumber] int lineNumber = default, [CallerFilePath] string? sourceFile = default) =>
            builder.OutFile(activity => activity
                    .Set(x => x.Data, data)
                    .Set(x => x.Path, path)
                    .Set(x => x.Mode, mode),
                lineNumber,
                sourceFile);

        public static IActivityBuilder OutFile(this IBuilder builder, Func<ValueTask<byte[]>> data, Func<ValueTask<string>> path, Func<ValueTask<CopyMode>> mode, [CallerLineNumber] int lineNumber = default, [CallerFilePath] string? sourceFile = default) =>
            builder.OutFile(activity => activity
                    .Set(x => x.Data, data)
                    .Set(x => x.Path, path)
                    .Set(x => x.Mode, mode),
                lineNumber,
                sourceFile);

        public static IActivityBuilder OutFile(this IBuilder builder, byte[] data, string path, CopyMode mode, [CallerLineNumber] int lineNumber = default, [CallerFilePath] string? sourceFile = default) =>
            builder.OutFile(activity => activity
                    .Set(x => x.Data, data)
                    .Set(x => x.Path, path)
                    .Set(x => x.Mode, mode),
                lineNumber,
                sourceFile);
    }
}
