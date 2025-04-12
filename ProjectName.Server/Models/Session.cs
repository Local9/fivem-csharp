using ProjectName.Server.Entities;

namespace ProjectName.Server.Models
{
    /// <summary>
    /// Session is used to store information about a player's session.
    /// Here you can store character, inventory, etc.
    /// </summary>
    public class Session
    {
        /// <summary>
        /// CFX Server Handle
        /// </summary>
        public int Handle { get; set; }

        /// <summary>
        /// Project User Information
        /// </summary>
        internal User User { get; private set; }

        public Session(int handle)
        {
            Handle = handle;
        }

        internal void SetUser(User user)
        {
            User = user;
        }
    }
}
