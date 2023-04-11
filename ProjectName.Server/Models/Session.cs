using ProjectName.Server.Database.Domain;

namespace ProjectName.Server.Models
{
    /// <summary>
    /// Session is used to store information about a player's session.
    /// Here you can store character, inventory, etc.
    /// </summary>
    public class Session
    {
        public int Handle { get; set; }
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
