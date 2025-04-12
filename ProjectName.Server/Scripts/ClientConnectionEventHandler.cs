using FxEvents;
using FxEvents.Shared.TypeExtensions;
using ProjectName.Server.Entities;
using ProjectName.Server.Models;
using ProjectName.Shared;

namespace ProjectName.Server.Scripts
{
    internal class ClientConnectionEventHandler : ScriptBase
    {
        private static readonly object _padlock = new();
        private static ClientConnectionEventHandler _instance;

        private ClientConnectionEventHandler()
        {
            AttachEvent("playerConnecting", new Action<Player, string, CallbackDelegate, dynamic>(OnPlayerConnectingAsync));

            EventDispatcher.Mount("connection:active", new Func<Player, Task<bool>>(OnConnectionActiveAsync));
            EventDispatcher.Mount("connection:ping", new Func<EventSource, Task<string>>(OnConnectionPingAsync));
        }

        internal static ClientConnectionEventHandler Instance
        {
            get
            {
                lock (_padlock)
                {
                    return _instance ??= new ClientConnectionEventHandler();
                }
            }
        }

        private async Task<string> OnConnectionPingAsync([FromSource] EventSource source)
        {
            // example of using the players information from the Active Session
            Logger.Debug($"Player {source.Player.Name} pinged the server.");
            Logger.Debug($"Players last name: {source.Session.User.LastNameUsed}.");
            // example of getting a ping response from the client
            string result = await EventDispatcher.Get<string>(source.Player, "client:ping");
            Logger.Debug($"Server pinged player '{source.Player.Name}' got result '{result}'.");
            return "pong";
        }

        private async Task<bool> OnConnectionActiveAsync([FromSource] Player player)
        {
            try
            {
                Logger.Info($"Player {player.Name} is active.");

                // example of setting a state bag value
                player.State.Set(StateBagKey.PlayerName, player.Name, true);

                User user = await User.GetUser(player);

                Session session = new(player.Handle.ToInt());
                session.SetUser(user);

                Main.ActiveSessions.TryAdd(session.Handle, session);

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error($"OnConnectionActiveAsync() Exception");
                Logger.Info($"{ex}");
                Logger.Error($"OnConnectionActiveAsync() Exception");
                return false;
            }
        }

        private async void OnPlayerConnectingAsync([FromSource] Player player, string playerName, CallbackDelegate kickReason, dynamic deferrals)
        {
            deferrals.defer();

            await BaseScript.Delay(100);
            Logger.Info($"Player {playerName} is connecting.");

            // Get player information from the database

            deferrals.done();
        }
    }
}
