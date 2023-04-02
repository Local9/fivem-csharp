using Logger;

namespace ProjectName.Server.Scripts
{
    internal class ScriptBase
    {
        /// <summary>
        /// FxEvents Logger.
        /// </summary>
        internal static Log Logger => Main.Logger;

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
        internal static void AttachTick(Func<Task> action) => Main.Instance.AttachTickHandler(action);

        /// <summary>
        /// Detaches a Tick.
        /// </summary>
        /// <param name="task"></param>
        internal static void DetachTick(Func<Task> action) => Main.Instance.DetachTickHandler(action);
    }
}
