using Logger;
using ProjectName.Server.Database;

namespace ProjectName.Server
{
    public class Main : BaseScript
    {
        internal static Main Instance { get; private set; }
        internal static PlayerList PlayerList { get; private set; }
        internal static ExportDictionary ExportDictionary { get; private set; }
        internal static Log Logger { get; private set; }
        internal static bool IsReady { get; private set; }

        public Main()
        {
            PlayerList = Players;
            ExportDictionary = Exports;
            Logger = new();

            Instance = this;

            OnLoadAsync();
        }

        /// <summary>
        /// OnLoadAsync is used to run async methods as they cannot be ran in the constructor.
        /// </summary>
        private async void OnLoadAsync()
        {
            try
            {
                await OnDatabaseTestAsync();

                _ = Scripts.ClientConnection.Instance;

                IsReady = true;
            }
            catch (Exception ex)
            {
                Logger.Error($"---------------------------------------------.");
                Logger.Error($"Server failed to load.");
                Logger.Info($"{ex}");
                Logger.Error($"---------------------------------------------.");
            }
        }

        /// <summary>
        /// Test the database connection.
        /// </summary>
        private async Task OnDatabaseTestAsync()
        {
            bool databaseTest = await Dapper<bool>.GetSingleAsync("select 1;");
            if (databaseTest)
                Logger.Info($"Database Connection Test Successful!");
            else
                Logger.Error($"Database Connection Test Failed!");
        }

        /// <summary>
        /// Awaitable tick handler for async methods to use to check if the server is ready.
        /// </summary>
        /// <returns></returns>
        internal static async Task IsReadyAsync()
        {
            while (!IsReady)
            {
                await BaseScript.Delay(100);
            }
        }

        /// <summary>
        /// Attaches a Tick.
        /// </summary>
        /// <param name="task"></param>
        internal void AttachTickHandler(Func<Task> task)
        {
            Tick += task;
        }

        /// <summary>
        /// Detaches a Tick.
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

        internal static async Task OnReturnToMainThreadAsync()
        {
            await Delay(0);
        }
    }
}