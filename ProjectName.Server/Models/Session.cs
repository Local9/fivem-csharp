using ProjectName.Server.Database.Domain;

namespace ProjectName.Server.Models
{
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
