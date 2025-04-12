using FxEvents;
using ProjectName.Server.Entities;
using ProjectName.Server.Models;
using ProjectName.Shared;

namespace ProjectName.Server.Scripts
{
    internal class UserEventHandler : ScriptBase
    {
        private static readonly object _padlock = new();
        private static UserEventHandler _instance;

        private UserEventHandler()
        {
            EventDispatcher.Mount("user:get:characters", new Func<EventSource, Task<IEnumerable<Character>>>(OnUserGetCharactersAsync));
            EventDispatcher.Mount("user:set:character", new Func<EventSource, int, Task<Character>>(OnUserSetCharacterAsync));
        }

        internal static UserEventHandler Instance
        {
            get
            {
                lock (_padlock)
                {
                    return _instance ??= new UserEventHandler();
                }
            }
        }

        private async Task<IEnumerable<Character>> OnUserGetCharactersAsync([FromSource] EventSource source)
        {
            return await source.Session.User.GetCharacters();
        }

        private async Task<Character> OnUserSetCharacterAsync([FromSource] EventSource source, int characterId)
        {
            return await source.Session.User.SetCharacter(characterId);
        }
    }
}
