namespace DynamicExec {
    using System.Diagnostics;
    using System.Threading.Tasks;
    using System;

    internal static class ProcessExtensions {
        public static TaskCompletionSource<object> Launch(this Process process, bool noEcho, string echoPrefix) {
            var tcs = new TaskCompletionSource<object>();
            process.Exited += (s, e) => tcs.SetResult(default);
            process.EnableRaisingEvents = true;
            process.Start();
            return tcs;
        }

        public static void Throw(this Process process) =>
            throw new NonZeroExitCodeException(process.ExitCode);
    }
}