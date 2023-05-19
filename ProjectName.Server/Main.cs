using FxEvents;
using Logger;
using ProjectName.Server.Database;
using ProjectName.Server.Models;
using ProjectName.Shared;
using System.Collections.Concurrent;

namespace ProjectName.Server
{
    public class Main : BaseScript
    {
        internal static Main Instance { get; private set; }
        internal static PlayerList PlayerList { get; private set; }
        internal static ConcurrentDictionary<int, Session> ActiveSessions = new();
        internal static Log Logger { get; private set; }
        internal static bool IsReady { get; private set; }

        internal Exports ExportDictionary => Exports;

        public Main()
        {
            Logger = new();
            PlayerList = new PlayerList();
            EventDispatcher.Initalize($"{FxEventKeys.FX_KEY_INBOUND}_rpc_in", $"{FxEventKeys.FX_KEY_OUTBOUND}_rpc_out", $"{FxEventKeys.FX_KEY_SIGNATURE}_sig");

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
                await DatabaseMigration.RunMigrations();
                await OnDatabaseTestAsync();

                _ = Scripts.ClientConnection.Instance;

                IsReady = true;
            }
            catch (Exception ex)
            {
                Logger.Error($"---------------------------------------------.");
                Logger.Error($"Server failed to load, reverted migrations.");
                Logger.Info($"{ex}");
                Logger.Error($"---------------------------------------------.");
            }
        }

        /// <summary>
        /// Test the database connection.
        /// </summary>
        private async Coroutine OnDatabaseTestAsync()
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
        internal static async Coroutine IsReadyAsync()
        {
            while (!IsReady)
            {
                await Wait(100);
            }
        }

        /// <summary>
        /// Attaches a Tick.
        /// </summary>
        /// <param name="task"></param>
        internal void AttachTick(Func<Coroutine> task)
        {
            Tick += task;
        }

        /// <summary>
        /// Detaches a Tick.
        /// </summary>
        /// <param name="task"></param>
        internal void DetachTick(Func<Coroutine> task)
        {
            Tick -= task;
        }

        /// <summary>
        /// Adds an event handler to the event handlers dictionary.
        /// </summary>
        /// <remarks>This event will not go through FxEvents</remarks>
        /// <param name="eventName"></param>
        /// <param name="delegate"></param>
        internal void AddEventHandler(string eventName, DynFunc @delegate, Binding binding = Binding.Local)
        {
            Logger.Debug($"Registered Event Handler '{eventName}'");
            EventHandlers[eventName].Add(@delegate, binding);
        }

        internal static async Coroutine OnReturnToMainThreadAsync()
        {
            await WaitUntilNextFrame();
        }

        /// <summary>
        /// Get the current session from the players server handle.
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public static Session ToSession(int handle)
        {
            return ActiveSessions.TryGetValue(handle, out Session user) ? user : null;
        }
    }
}