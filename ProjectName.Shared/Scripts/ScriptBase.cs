using Logger;
#if Server
using ProjectName.Server;
#elif Client
using ProjectName.Client;
using ProjectName.Client.Scripts.GameEvent;
#endif

namespace ProjectName.Shared
{
    internal class ScriptBase
    {
        /// <summary>
        /// FxEvents Logger.
        /// </summary>
        internal static Log Logger => Main.Logger;

        /// <summary>
        /// CFX Exports
        /// </summary>
        internal static ExportDictionary Exports => Main.Instance.ExportDictionary;

        /// <summary>
        /// Adds an event handler to the event handlers dictionary.
        /// </summary>
        /// <remarks>This event will not go through FxEvents</remarks>
        /// <param name="eventName"></param>
        /// <param name="delegate"></param>
        internal static void AttachEvent(string eventName, Delegate action) => Main.Instance.AddEventHandler(eventName, action);

        /// <summary>
        /// Attaches a Tick.
        /// </summary>
        /// <param name="task"></param>
        internal static void AttachTick(Func<Task> action) => Main.Instance.AttachTick(action);

        /// <summary>
        /// Detaches a Tick.
        /// </summary>
        /// <param name="task"></param>
        internal static void DetachTick(Func<Task> action) => Main.Instance.DetachTick(action);

#if Client
        /// <summary>
        /// Access to the EventNetworkEntityDamage class.
        /// </summary>
        internal static EventNetworkEntityDamage EventNetworkEntityDamage => Main.Instance.EventNetworkEntityDamage;
#endif
    }
}
