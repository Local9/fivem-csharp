using FxEvents.Shared.EventSubsystem;

namespace ProjectName.Server.Models
{
    /// <summary>
    /// EventSource is used by FxEvents to allow you to pull information needed for your events.
    /// </summary>
    public partial class EventSource : ISource
    {
        /// <summary>
        /// CFX Server Handle
        /// </summary>
        public int Handle { get; set; }

        /// <summary>
        /// CFX Player Information (Name, Ping, etc.)
        /// </summary>
        internal CitizenFX.Core.Player Player => Main.PlayerList[Handle];

        /// <summary>
        /// Session Information (User, Character, etc.)
        /// </summary>
        internal Session Session { get; private set; }

        public EventSource()
        {

        }

        public EventSource(int handle)
        {
            Handle = handle;
            if (handle > 0)
                Session = Main.ToSession(handle);
        }

        public override string ToString()
        {
            return $"{Handle} ({Player.Name})";
        }

        public static explicit operator EventSource(string netId)
        {
            if (int.TryParse(netId.Replace("net:", string.Empty), out int handle))
            {
                return new EventSource(handle);
            }

            throw new Exception($"Could not parse net id: {netId}");
        }

        public bool Compare(EventSource client)
        {
            return client.Handle == Handle;
        }

        public static explicit operator EventSource(int handle) => new(handle);
    }
}
