using Dapper;
using ProjectName.Server.Database;

namespace ProjectName.Server.Entities
{
    public partial class Character
    {
        [Description("id")]
        public int Id { get; private set; }

        [Description("user_id")]
        public int UserId { get; private set; }

        [Description("name")]
        public string Name { get; private set; }

        public static async Task<IEnumerable<Character>> GetCharactersByUserId(int userId)
        {
            DynamicParameters dynamicParameters = new();
            dynamicParameters.Add("pUserId", userId);

            return await Dapper<Character>.QueryAsync("call selCharactersByUserId(@pUserId);", dynamicParameters);
        }
    }
}
