namespace DynamicExec
{
    using System.Reflection;

    internal static class DefaultPrefix
    {
        static DefaultPrefix()
        {
        }

        public static readonly string Value = Assembly.GetEntryAssembly()?.GetName().Name ?? "DynamicExec";
    }
}
