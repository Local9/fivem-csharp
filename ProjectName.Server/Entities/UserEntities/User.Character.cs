namespace ProjectName.Server.Entities
{
    public partial class User
    {
        private Character _activeCharacter;

        public Character ActiveCharacter
        {
            get => _activeCharacter;
            set
            {
                _activeCharacter = value;
            }
        }

        public async Task<IEnumerable<Character>> GetCharacters()
        {
            return await Character.GetCharactersByUserId(Id);
        }

        public async Task<Character> SetCharacter(int characterId)
        {
            IEnumerable<Character> characters = await GetCharacters();
            Character character = characters.FirstOrDefault(x => x.Id == characterId);
            if (character is null)
            {
                throw new Exception("Character not found");
            }
            ActiveCharacter = character;
            return ActiveCharacter;
        }
    }
}
