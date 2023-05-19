using Dapper;
using FxEvents.Shared;

namespace ProjectName.Server.Database.Domain
{
    public class User
    {
        [Description("id")]
        public int Id { get; private set; }

        [Description("last_name")]
        public string LastName { get; private set; }

        [Description("created")]
        public DateTime Created { get; private set; }

        [Description("last_seen")]
        public DateTime LastSeen { get; private set; }

        public static async Coroutine<User> GetUser(CitizenFX.Core.Player player)
        {
            // Get player tokens
            CString handle = $"{player.Handle}";
            int numTokens = Natives.GetNumPlayerTokens(handle);
            List<string> tokens = new();
            for (int i = 0; i < numTokens; i++)
            {
                tokens.Add(Natives.GetPlayerToken(handle, i));
            }

            // Get player identifiers
            int numIdentifiers = Natives.GetNumPlayerIdentifiers(handle);
            List<string> identifiers = new();
            for (int i = 0; i < numIdentifiers; i++)
            {
                string identifier = Natives.GetPlayerIdentifier(handle, i);

                if (identifier.StartsWith("ip:")) continue;

                identifiers.Add(Natives.GetPlayerIdentifier(handle, i));
            }

            string fivem = identifiers.FirstOrDefault(x => x.StartsWith("fivem:"));
            string steam = identifiers.FirstOrDefault(x => x.StartsWith("steam:"));
            string discord = identifiers.FirstOrDefault(x => x.StartsWith("discord:"));
            string license2 = identifiers.FirstOrDefault(x => x.StartsWith("license2:"));
            string license = identifiers.FirstOrDefault(x => x.StartsWith("license:"));
            string live = identifiers.FirstOrDefault(x => x.StartsWith("live:"));
            string xbl = identifiers.FirstOrDefault(x => x.StartsWith("xbl:"));

            // find user using fivem identifier
            User user = await OnGetUserByIdentityAsync(fivem);
            if (user is not null)
            {
                Main.Logger.Debug($"Found user {user.LastName} by fivem identifier.");
                return user;
            }
            // find user using steam identifier
            user = await OnGetUserByIdentityAsync(steam);
            if (user is not null)
            {
                Main.Logger.Debug($"Found user {user.LastName} by steam identifier.");
                return user;
            }
            // find user using discord identifier
            user = await OnGetUserByIdentityAsync(discord);
            if (user is not null)
            {
                Main.Logger.Debug($"Found user {user.LastName} by discord identifier.");
                return user;
            }
            // find user using license2 identifier
            user = await OnGetUserByIdentityAsync(license2);
            if (user is not null)
            {
                Main.Logger.Debug($"Found user {user.LastName} by license2 identifier.");
                return user;
            }
            // find user using license identifier
            user = await OnGetUserByIdentityAsync(license);
            if (user is not null)
            {
                Main.Logger.Debug($"Found user {user.LastName} by license identifier.");
                return user;
            }
            // find user using xbl identifier
            user = await OnGetUserByIdentityAsync(xbl);
            if (user is not null)
            {
                Main.Logger.Debug($"Found user {user.LastName} by xbl identifier.");
                return user;
            }
            // find user using live identifier
            user = await OnGetUserByIdentityAsync(live);
            if (user is not null)
            {
                Main.Logger.Debug($"Found user {user.LastName} by live identifier.");
                return user;
            }

            // find user using tokens
            user = await OnGetUserByTokensAsync(tokens);
            if (user is not null)
            {
                Main.Logger.Debug($"Found user {user.LastName} by token.");
                return user;
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

        private static async Coroutine<User> OnInsertUserAsync(string lastName)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("pLastName", lastName);

            return await Dapper<User>.GetSingleAsync("call insUser(@pLastName);", dynamicParameters);
        }

        private async Coroutine OnInsertTokenAsync(string token)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("pUserId", Id);
            dynamicParameters.Add("pToken", token);

            await Dapper<User>.ExecuteAsync("call insUserToken(@pUserId, @pToken);", dynamicParameters);
        }

        private async Coroutine OnInsertIdentityAsync(string type, string value)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("pUserId", Id);
            dynamicParameters.Add("pType", type);
            dynamicParameters.Add("pValue", value);

            await Dapper<User>.ExecuteAsync("call insUserIdentity(@pUserId, @pType, @pValue);", dynamicParameters);
        }

        private static async Coroutine<User> OnGetUserByIdentityAsync(string identity)
        {
            if (string.IsNullOrEmpty(identity)) return null;

            string[] ident = identity.Split(':');
            string type = ident[0];
            string value = ident[1];

            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("pType", type);
            dynamicParameters.Add("pValue", value);

            return await Dapper<User>.GetSingleAsync("call selUserByIdentity(@pType, @pValue);", dynamicParameters);
        }

        private static async Coroutine<User> OnGetUserByTokensAsync(List<string> tokens)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("pTokens", string.Join(",", tokens));

            return await Dapper<User>.GetSingleAsync("call selUserByToken(@pTokens);", dynamicParameters);
        }

        public override string ToString()
        {
            return this.ToJson();
        }
    }
}
