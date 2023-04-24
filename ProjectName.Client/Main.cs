using FxEvents;
using Logger;
using ProjectName.Shared;

namespace ProjectName.Client
{
    public class Main : BaseScript
    {
        internal static Main Instance { get; private set; }
        internal static Log Logger { get; private set; }
        internal static PlayerList PlayerList { get; private set; }
        public static int GameTime { get; private set; }

        public Main()
        {
            PlayerList = Players;
            Logger = new();
            EventDispatcher.Initalize($"{FxEventKeys.FX_KEY_INBOUND}_rpc_in", $"{FxEventKeys.FX_KEY_OUTBOUND}_rpc_out", $"{FxEventKeys.FX_KEY_SIGNATURE}_sig");

            Instance = this;

            InitialiseScripts();
        }

        private void InitialiseScripts()
        {
            _ = Scripts.ClientConnection.Instance;
        }

        /// <summary>
        /// Attaches a Tick
        /// </summary>
        /// <param name="task"></param>
        internal void AttachTickHandler(Func<Task> task)
        {
            Tick += task;
        }

        /// <summary>
        /// Detaches a Tick
        /// </summary>
        /// <param name="task"></param>
        internal void DetachTickHandler(Func<Task> task)
        {
            Tick -= task;
        }

        /// <summary>
        /// Adds an event handler to the event handlers dictionary.
        /// </summary>
        /// <remarks>This event will not go through FxEvents</remarks>
        /// <param name="eventName"></param>
        /// <param name="delegate"></param>
        internal void AddEventHandler(string eventName, Delegate @delegate)
        {
            Logger.Debug($"Registered Event Handler '{eventName}'");
            EventHandlers[eventName] += @delegate;
        }

        /// <summary>
        /// Gets the current game timer. This is the time in seconds since the game started.
        /// Using this as the only method to call GetGameTimer will lower the amount of calls to the native.
        /// </summary>
        [Tick]
        private async Task OnUpdateGameTimerAsync()
        {
            GameTime = GetGameTimer();
            await BaseScript.Delay(500);
        }
    }
}