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

        public Session(int handle)
        {
            Handle = handle;
        }
    }
}
