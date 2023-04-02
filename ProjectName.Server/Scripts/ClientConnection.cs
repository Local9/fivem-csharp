using FxEvents;
using ProjectName.Shared;

namespace ProjectName.Server.Scripts
{
    internal class ClientConnection : ScriptBase
    {
        internal static ClientConnection Instance { get; private set; } = new ClientConnection();

        private ClientConnection()
        {
            AttachEvent("playerConnecting", new Action<Player, string, CallbackDelegate, dynamic>(OnPlayerConnectingAsync));

            EventDispatcher.Mount("connection:active", new Func<Player, Task<bool>>(OnConnectionActiveAsync));
        }

        private async Task<bool> OnConnectionActiveAsync([FromSource] Player player)
        {
            Logger.Info($"Player {player.Name} is active.");

            // example of setting a state bag value
            player.State.Set(StateBagKey.PlayerName, player.Name, true);

            return true;
        }

        private async void OnPlayerConnectingAsync([FromSource] Player player, string playerName, CallbackDelegate kickReason, dynamic deferrals)
        {
            deferrals.defer();

            await BaseScript.Delay(100);
            Logger.Info($"Player {playerName} is connecting.");

            deferrals.done();
        }
    }
}
