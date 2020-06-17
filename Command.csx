using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

public static readonly string DefaultPrefix = Assembly.GetEntryAssembly()?.GetName().Name ?? "DynamicExec";

/// <summary>
/// Contains methods for running commands and reading standard output (stdout).
/// </summary>
public static class Command {
    public static ProcessStartInfo CreateProcessStartInfo(
        string name, string args, string workingDirectory, bool captureOutput, string windowsName, string windowsArgs, Action<IDictionary<string, string>> configureEnvironment) {
        var startInfo = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
            new ProcessStartInfo {
                FileName = windowsName ?? name,
                Arguments = windowsArgs ?? args,
                WorkingDirectory = workingDirectory,
                UseShellExecute = false,
                RedirectStandardError = false,
                RedirectStandardOutput = captureOutput
                } :
                new ProcessStartInfo {
                FileName = name,
                Arguments = args,
                WorkingDirectory = workingDirectory,
                UseShellExecute = false,
                RedirectStandardError = false,
                RedirectStandardOutput = captureOutput
            };

        configureEnvironment?.Invoke(startInfo.Environment);

        return startInfo;
    }
    /// <summary>
    /// Runs a command asynchronously.
    /// By default, the command line is echoed to standard error (stderr).
    /// </summary>
    /// <param name="name">The name of the command. This can be a path to an executable file.</param>
    /// <param name="args">The arguments to pass to the command.</param>
    /// <param name="workingDirectory">The working directory in which to run the command.</param>
    /// <param name="noEcho">Whether or not to echo the resulting command line and working directory (if specified) to standard error (stderr).</param>
    /// <param name="windowsName">The name of the command to use on Windows only.</param>
    /// <param name="windowsArgs">The arguments to pass to the command on Windows only.</param>
    /// <param name="echoPrefix">The prefix to use when echoing the command line and working directory (if specified) to standard error (stderr).</param>
    /// <param name="configureEnvironment">An action which configures environment variables for the command.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous running of the command.</returns>
    /// <exception cref="NonZeroExitCodeException">The command exited with non-zero exit code.</exception>
    /// <remarks>
    /// By default, the resulting command line and the working directory (if specified) are echoed to standard error (stderr).
    /// To suppress this behavior, provide the <paramref name="noEcho"/> parameter with a value of <c>true</c>.
    /// </remarks>
    public static TaskCompletionSource<object> Launch(string name, string args = null, string workingDirectory = null, bool noEcho = false, string windowsName = null, string windowsArgs = null, string echoPrefix = null, Action<IDictionary<string, string>> configureEnvironment = null) {
        using var process = new Process();
        var prefix = echoPrefix ?? DefaultPrefix;
        var info = CreateProcessStartInfo(name, args, workingDirectory, false, windowsName, windowsArgs, configureEnvironment);
        if (!noEcho) {
            var message = $"{(string.IsNullOrEmpty(info.WorkingDirectory) ? "" : $"{prefix}: Working directory: {info.WorkingDirectory}{Environment.NewLine}")}{prefix}: {info.FileName} {info.Arguments}";
            Console.Error.WriteLine(message);
        }
        var tcs = new TaskCompletionSource<object>();
        process.Exited += (s, e) => tcs.SetResult(default);
        process.EnableRaisingEvents = true;
        process.Start();
        tcs.Task.ConfigureAwait(false);
        return tcs;

    }
}
