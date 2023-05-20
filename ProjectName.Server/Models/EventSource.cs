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
        internal Player Player { get; private set; }

        /// <summary>
        /// Session Information (User, Character, etc.)
        /// </summary>
        internal Session Session { get; private set; }

        public EventSource()
        {

        }

        public EventSource(Remote remote)
        {
            Player = (Player)remote;
            Handle = Player.Handle;
            Session = Main.ToSession(Player.Handle);
        }

        public override string ToString()
        {
            return $"{Handle} ({Player.Name})";
        }

        public bool Compare(EventSource client)
        {
            return client.Handle == Handle;
        }

        public static explicit operator EventSource(Remote handle) => new(handle);
    }
}
