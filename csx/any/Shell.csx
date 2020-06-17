namespace DynamicExec
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;

    /// <summary>
    /// A dynmaic object that let you run commands like they are C# Methods
    ///</summary>
    public class Shell : DynamicObject
    {
        private string workingDirectory;
        private bool noEcho;
        private string windowsName;
        private string windowsArgs;
        private string echoPrefix;
        private Action<IDictionary<string, string>> configureEnvironment;
        private bool async;

        /// <summary>
        /// Creates a new Shell context to run commands from.
        /// By default, the command line is echoed to standard error (stderr).
        /// </summary>
        /// <param name="workingDirectory">The working directory in which each command will run.</param>
        /// <param name="noEcho">Whether or not to echo the resulting command line and working directory (if specified) to standard error (stderr).</param>
        /// <param name="async">Will all commands be executed async</param>
        /// <param name="windowsName">The name of the command to use on Windows only.</param>
        /// <param name="windowsArgs">The arguments to pass to the command on Windows only.</param>
        /// <param name="echoPrefix">The prefix to use when echoing the command line and working directory (if specified) to standard error (stderr).</param>
        /// <param name="configureEnvironment">An action which configures environment variables for the command.</param>
        /// <remarks>
        /// By default, the resulting command line and the working directory (if specified) are echoed to standard error (stderr).
        /// To suppress this behavior, provide the <paramref name="noEcho"/> parameter with a value of <c>true</c>.
        /// </remarks>
        public Shell(string workingDirectory = null, bool noEcho = false, string windowsName = null, string windowsArgs = null, string echoPrefix = null, Action<IDictionary<string, string>> configureEnvironment = null)
        {
            this.workingDirectory = workingDirectory;
            this.noEcho = noEcho;
            this.windowsName = windowsName;
            this.windowsArgs = windowsArgs;
            this.echoPrefix = echoPrefix;
            this.configureEnvironment = configureEnvironment;
        }
        /// <summary>
        /// The dynamic command dispatcher
        /// </summary>
        /// <param name="binder">description</param>
        /// <param name="args">description</param>
        /// <param name="result">description</param>
        /// <returns>A <see cref="string"/> representing the contents of standard output (stdout).</returns>
        /// <exception cref="NonZeroExitCodeException">The command exited with non-zero exit code.</exception>
        public override bool TryInvokeMember(InvokeMemberBinder binder,
                                             object[] args,
                                             out object result)
        {
            if (binder == null)
            {
                throw new ArgumentNullException(nameof(binder));
            }
            string commandline = null;

            if (args != null && args.Length > 0)
            {
                commandline = args.Where(x => x != null)
                                .Select(x => x.ToString())
                                .Aggregate((a, b) => $"{a} {b}");
            }


            result = Command.Launch($"{binder.Name}.exe", commandline, workingDirectory,noEcho,windowsName,windowsArgs,echoPrefix,configureEnvironment);
            return true;
        }
    }

}
