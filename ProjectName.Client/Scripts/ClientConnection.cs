using FxEvents;
using ProjectName.Shared;

namespace ProjectName.Client.Scripts
{
    internal sealed class ClientConnection : ScriptBase
    {
        internal static ClientConnection Instance { get; private set; } = new ClientConnection();

        private ClientConnection()
        {
            OnStartupAsync();
            EventDispatcher.Mount("client:ping", new Func<Task<string>>(OnClientPingAsync));
        }

        private async Task<string> OnClientPingAsync()
        {
            return "pong";
        }

        internal async void OnStartupAsync()
        {
            try
            {
                bool isConnectionActive = await EventDispatcher.Get<bool>("connection:active");
                if (isConnectionActive)
                    Logger.Info("Connection is active.");
                else
                    Logger.Info("Connection is not active.");

                string ping = await EventDispatcher.Get<string>("connection:ping");
                Logger.Info($"Ping: {ping}");
            }
            catch (Exception ex)
            {
                Logger.Error($"---------------------------------------------.");
                Logger.Error($"Client failed to load.");
                Logger.Info($"{ex}");
                Logger.Error($"---------------------------------------------.");
            }
        }
    }
}
