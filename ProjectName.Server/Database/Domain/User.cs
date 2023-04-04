using Dapper;
using FxEvents.Shared;

namespace ProjectName.Server.Database.Domain
{
    public class User
    {
        [Description("UserId")]
        public int UserId { get; private set; }

        [Description("LastName")]
        public string LastName { get; private set; }

        [Description("Created")]
        public DateTime Created { get; private set; }

        public static async Task<User> GetUser(Player player)
        {
            // Get player tokens
            int numTokens = GetNumPlayerTokens(player.Handle);
            List<string> tokens = new();
            for (int i = 0; i < numTokens; i++)
            {
                tokens.Add(GetPlayerToken(player.Handle, i));
            }

            // Get player identifiers
            int numIdentifiers = GetNumPlayerIdentifiers(player.Handle);
            List<string> identifiers = new();
            for (int i = 0; i < numIdentifiers; i++)
            {
                identifiers.Add(GetPlayerIdentifier(player.Handle, i));
            }

            // find user using tokens
            User user = await OnGetUserByTokensAsync(tokens);
            if (user is not null) return user;

            // find user using identifiers
            foreach (string identity in identifiers)
            {
                user = await OnGetUserByIdentityAsync(identity);
                if (user is not null) return user;
            }

            // if no user is returned, then create one.
            if (user is null)
            {
                user = await OnInsertUserAsync(player.Name);

                foreach (string token in tokens)
                    await user.OnInsertTokenAsync(token);

                foreach (string identity in identifiers)
                {
                    string[] ident = identity.Split(':');
                    await user.OnInsertIdentityAsync(ident[0], ident[1]);
                }

                return user;
            }

            return default;
        }

        private static async Task<User> OnInsertUserAsync(string lastName)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("pLastName", lastName);

            return await Dapper<User>.GetSingleAsync("insUser", dynamicParameters);
        }

        private async Task OnInsertTokenAsync(string token)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("pUserId", UserId);
            dynamicParameters.Add("pToken", token);

            await Dapper<User>.ExecuteAsync("insUserToken", dynamicParameters);
        }

        private async Task OnInsertIdentityAsync(string type, string value)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("pUserId", UserId);
            dynamicParameters.Add("pType", type);
            dynamicParameters.Add("pValue", value);

            await Dapper<User>.ExecuteAsync("insUserIdentity", dynamicParameters);
        }

        private static async Task<User> OnGetUserByIdentityAsync(string identity)
        {
            string[] ident = identity.Split(':');
            string type = ident[0];
            string value = ident[1];

            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("pType", type);
            dynamicParameters.Add("pValue", value);

            return await Dapper<User>.GetSingleAsync("selUserByIdentity", dynamicParameters);
        }

        private static async Task<User> OnGetUserByTokensAsync(List<string> tokens)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("pTokens", string.Join(",", tokens));

            return await Dapper<User>.GetSingleAsync("selUserByToken", dynamicParameters);
        }

        public override string ToString()
        {
            return this.ToJson();
        }
    }
}
