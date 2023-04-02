using FxEvents;

namespace ProjectName.Client.Scripts
{
    internal sealed class ClientConnection : ScriptBase
    {
        internal static ClientConnection Instance { get; private set; } = new ClientConnection();

        private ClientConnection()
        {
            OnStartupAsync();
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
