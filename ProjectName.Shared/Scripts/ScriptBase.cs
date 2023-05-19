using Logger;
#if Server
using ProjectName.Server;
#elif Client
using ProjectName.Client;
#endif

namespace ProjectName.Shared
{
    /// <summary>
    /// Base class for all scripts, contains common functionality which is shared on both the client and server.
    /// </summary>
    internal class ScriptBase
    {
        /// <summary>
        /// FxEvents Logger.
        /// </summary>
        internal static Log Logger => Main.Logger;

        /// <summary>
        /// CFX Exports
        /// </summary>
        internal static Exports ExportDictionary => Main.Instance.ExportDictionary;

        /// <summary>
        /// Adds an event handler to the event handlers dictionary.
        /// </summary>
        /// <remarks>This event will not go through FxEvents</remarks>
        /// <param name="eventName"></param>
        /// <param name="delegate"></param>
        internal static void AttachEvent(string eventName, DynFunc action, Binding binding = Binding.Local) => Main.Instance.AddEventHandler(eventName, action, binding);

        /// <summary>
        /// Attaches a Tick.
        /// </summary>
        /// <param name="task"></param>
        internal static void AttachTick(Func<Coroutine> action) => Main.Instance.AttachTick(action);

        /// <summary>
        /// Detaches a Tick.
        /// </summary>
        /// <param name="task"></param>
        internal static void DetachTick(Func<Coroutine> action) => Main.Instance.DetachTick(action);
    }
}
