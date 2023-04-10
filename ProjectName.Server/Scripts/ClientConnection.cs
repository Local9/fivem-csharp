using FxEvents;
using ProjectName.Server.Database.Domain;
using ProjectName.Shared;
using System.Collections.Concurrent;

namespace ProjectName.Server.Scripts
{
    internal class ClientConnection : ScriptBase
    {
        internal static ClientConnection Instance { get; private set; } = new ClientConnection();

        internal ConcurrentDictionary<string, User> ActiveUsers = new();

        private ClientConnection()
        {
            AttachEvent("playerConnecting", new Action<Player, string, CallbackDelegate, dynamic>(OnPlayerConnectingAsync));

            EventDispatcher.Mount("connection:active", new Func<Player, Task<bool>>(OnConnectionActiveAsync));
        }

        private async Task<bool> OnConnectionActiveAsync([FromSource] Player player)
        {
            try
            {
                Logger.Info($"Player {player.Name} is active.");

                // example of setting a state bag value
                player.State.Set(StateBagKey.PlayerName, player.Name, true);

                User user = await User.GetUser(player);
                ActiveUsers.TryAdd(player.Handle, user);

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
