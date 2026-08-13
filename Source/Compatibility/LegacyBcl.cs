using System;
using System.Threading;

namespace FactionLens.Compatibility
{
    internal static class LegacyBcl
    {
        internal static IDisposable Enter(object sync)
        {
            return new MonitorScope(sync);
        }

        internal static bool IsNullOrWhiteSpace(string value)
        {
#if RWT_LEGACY_BCL
            if (value == null) return true;
            for (int index = 0; index < value.Length; index++)
            {
                if (!char.IsWhiteSpace(value[index])) return false;
            }

            return true;
#else
            return string.IsNullOrWhiteSpace(value);
#endif
        }
    }

    internal sealed class MonitorScope : IDisposable
    {
        private readonly object sync;

        internal MonitorScope(object sync)
        {
            this.sync = sync;
            Monitor.Enter(sync);
        }

        public void Dispose()
        {
            Monitor.Exit(sync);
        }
    }
}
