using FxEvents;
using ProjectName.Server.Database.Domain;
using ProjectName.Server.Models;
using ProjectName.Shared;

namespace ProjectName.Server.Scripts
{
    internal class ClientConnection : ScriptBase
    {
        private static readonly object _padlock = new();
        private static ClientConnection _instance;

        private ClientConnection()
        {
            AttachEvent("playerConnecting", Func.Create<Player, string, Callback, dynamic>(OnPlayerConnectingAsync));
            AttachEvent("playerJoining", Func.Create<Player>(OnPlayerJoiningAsync));

            EventDispatcher.Mount("connection:active", Func.Create<Player, Coroutine<bool>>(OnConnectionActiveAsync));
            EventDispatcher.Mount("connection:ping", Func.Create<EventSource, Coroutine<string>>(OnConnectionPingAsync));
        }

        private void OnPlayerJoiningAsync([Source] Player player)
        {
            Logger.Info($"Player {player.Name} is joining.");
        }

        internal static ClientConnection Instance
        {
            get
            {
                lock (_padlock)
                {
                    return _instance ??= new ClientConnection();
                }
            }
        }

        private async Coroutine<string> OnConnectionPingAsync([Source] EventSource session)
        {
            // example of using the players information from the Active Session
            Logger.Debug($"Player {session.Player.Name} pinged the server.");
            Logger.Debug($"Players last name: {session.Session.User.LastName}.");
            // example of getting a ping response from the client
            string result = await EventDispatcher.Get<string>(session.Player, "client:ping");
            Logger.Debug($"Server pinged player '{session.Player.Name}' got result '{result}'.");
            return "pong";
        }

        private async Coroutine<bool> OnConnectionActiveAsync([Source] Player player)
        {
            try
            {
                Logger.Info($"Player {player.Name} is active.");

                // example of setting a state bag value
                player.State.Set(StateBagKey.PlayerName, player.Name, true);

                User user = await User.GetUser(player);

                Session session = new(player.Handle);
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

        private async void OnPlayerConnectingAsync([Source] Player player, string playerName, Callback kickReason, dynamic deferrals)
        {
            deferrals.defer();

            await BaseScript.Wait(100);
            Logger.Info($"Player {playerName} is connecting.");

            // Get player information from the database

            deferrals.done();
        }
    }
}
