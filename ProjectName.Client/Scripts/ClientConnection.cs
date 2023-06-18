using FxEvents;
using ProjectName.Shared;

namespace ProjectName.Client.Scripts
{
    internal sealed class ClientConnection : ScriptBase
    {
        private static readonly object _padlock = new();
        private static ClientConnection _instance;

        private ClientConnection()
        {
            OnStartupAsync();
            EventDispatcher.Mount("client:ping", Func.Create<Remote, Coroutine<string>>(OnClientPingAsync));
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

        private async Coroutine<string> OnClientPingAsync([Source] Remote remote)
        {
            return "pong";
        }

        internal async void OnStartupAsync()
        {
            try
            {
                string ping = await EventDispatcher.Get<string>("connection:ping");
                Logger.Info($"connection:ping: {ping}");
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
