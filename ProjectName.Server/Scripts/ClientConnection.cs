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
