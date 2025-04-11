using Dapper;
using FxEvents.Shared;

namespace ProjectName.Server.Database.Domain
{
    public class User
    {
        [Description("id")]
        public int Id { get; private set; }

        [Description("last_name_used")]
        public string LastNameUsed { get; private set; }

        [Description("created")]
        public DateTime Created { get; private set; }

        [Description("last_seen")]
        public DateTime LastSeen { get; private set; }

        /// <summary>
        /// Retrieves the user associated with the given player, creating a new user if one does not exist.
        /// </summary>
        /// <param name="player">This is a FiveM Player Class, it is used to obtain the users data or to create them if they do not exist.</param>
        /// <returns>The user associated with the player.</returns>
        /// <remarks>This method is static and can be called without an instance of the class.</remarks>
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
                string identifier = GetPlayerIdentifier(player.Handle, i);

                if (identifier.StartsWith("ip:")) continue;

                identifiers.Add(GetPlayerIdentifier(player.Handle, i));
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
                Main.Logger.Debug($"Found user {user.LastNameUsed} by fivem identifier.");
                return user;
            }
            // find user using steam identifier
            user = await OnGetUserByIdentityAsync(steam);
            if (user is not null)
            {
                Main.Logger.Debug($"Found user {user.LastNameUsed} by steam identifier.");
                return user;
            }
            // find user using discord identifier
            user = await OnGetUserByIdentityAsync(discord);
            if (user is not null)
            {
                Main.Logger.Debug($"Found user {user.LastNameUsed} by discord identifier.");
                return user;
            }
            // find user using license2 identifier
            user = await OnGetUserByIdentityAsync(license2);
            if (user is not null)
            {
                Main.Logger.Debug($"Found user {user.LastNameUsed} by license2 identifier.");
                return user;
            }
            // find user using license identifier
            user = await OnGetUserByIdentityAsync(license);
            if (user is not null)
            {
                Main.Logger.Debug($"Found user {user.LastNameUsed} by license identifier.");
                return user;
            }
            // find user using xbl identifier
            user = await OnGetUserByIdentityAsync(xbl);
            if (user is not null)
            {
                Main.Logger.Debug($"Found user {user.LastNameUsed} by xbl identifier.");
                return user;
            }
            // find user using live identifier
            user = await OnGetUserByIdentityAsync(live);
            if (user is not null)
            {
                Main.Logger.Debug($"Found user {user.LastNameUsed} by live identifier.");
                return user;
            }

            // find user using tokens
            user = await OnGetUserByTokensAsync(tokens);
            if (user is not null)
            {
                Main.Logger.Debug($"Found user {user.LastNameUsed} by token.");
                return user;
            }

            // As no user was found using the identifiers or tokens, we will create a new user.
            if (user is null)
            {
                // We will create a new user using the last name used by the player.
                user = await OnInsertUserAsync(player.Name);

                // We will insert the tokens and identifiers into the database.
                foreach (string token in tokens)
                    await user.OnInsertTokenAsync(token);

                // We will insert the identifiers into the database.
                foreach (string identity in identifiers)
                {
                    string[] ident = identity.Split(':');
                    await user.OnInsertIdentityAsync(ident[0], ident[1]);
                }

                Main.Logger.Debug($"Created new user {user.LastNameUsed} with id {user.Id}.");
                return user;
            }

            return default;
        }

        /// <summary>
        /// Inserts a new user into the database with the given last name used. The name is not unique and is only used to create a new user record.
        /// </summary>
        /// <param name="lastNameUsed">The last name used by the user.</param>
        /// <returns>The newly created user.</returns>
        private static async Task<User> OnInsertUserAsync(string lastNameUsed)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("pLastNameUsed", lastNameUsed);

            return await Dapper<User>.GetSingleAsync("call insUser(@pLastNameUsed);", dynamicParameters);
        }

        /// <summary>
        /// Inserts a new token for the user into the database.
        /// </summary>
        /// <param name="token">The token to insert.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>This method is private and should only be called from within the class.</remarks>
        private async Task OnInsertTokenAsync(string token)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("pUserId", Id);
            dynamicParameters.Add("pToken", token);

            await Dapper<User>.ExecuteAsync("call insUserToken(@pUserId, @pToken);", dynamicParameters);
        }

        /// <summary>
        /// Inserts a new identity for the user into the database.
        /// </summary>
        /// <param name="type">The type of identity (e.g., fivem, steam).</param>
        /// <param name="value">The value of the identity.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>This method is private and should only be called from within the class.</remarks>
        private async Task OnInsertIdentityAsync(string type, string value)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("pUserId", Id);
            dynamicParameters.Add("pType", type);
            dynamicParameters.Add("pValue", value);

            await Dapper<User>.ExecuteAsync("call insUserIdentity(@pUserId, @pType, @pValue);", dynamicParameters);
        }

        /// <summary>
        /// Retrieves a user from the database using the given identity.
        /// </summary>
        /// <param name="identity">The identity to search for.</param>
        /// <returns>The user associated with the identity, or null if not found.</returns>
        /// <remarks>This method is private and should only be called from within the class.</remarks>
        private static async Task<User> OnGetUserByIdentityAsync(string identity)
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

        /// <summary>
        /// Retrieves a user from the database using the given tokens.
        /// </summary>
        /// <param name="tokens">The list of tokens to search for.</param>
        /// <returns>The user associated with the tokens, or null if not found.</returns>
        /// <remarks>This method is private and should only be called from within the class.</remarks>
        private static async Task<User> OnGetUserByTokensAsync(List<string> tokens)
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
