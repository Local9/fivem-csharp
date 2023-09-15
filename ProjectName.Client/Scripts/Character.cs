using ProjectName.Client.Extension;
using ProjectName.Client.UI;
using ProjectName.Shared;

namespace ProjectName.Client.Scripts
{
    internal class Character : ScriptBase
    {
        private static readonly object _padlock = new();
        private static Character _instance;

        private Character()
        {
            EventNetworkEntityDamage.OnDeath += OnDeath;
        }

        internal static Character Instance
        {
            get
            {
                lock (_padlock)
                {
                    return _instance ??= new Character();
                }
            }
        }

        /// <summary>
        /// Handles the death of the player and will respawn them.
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="isMeleeDamage"></param>
        /// <param name="weaponHashInfo"></param>
        /// <param name="damageTypeFlag"></param>
        private async void OnDeath(Entity attacker, bool isMeleeDamage, uint weaponHashInfo, int damageTypeFlag)
        {
            await ScreenInterface.FadeOut();
            Game.PlayerPed.IsInvincible = true;

            Vector3 spawnLocation = Game.PlayerPed.Position;
            float heading = Game.PlayerPed.Heading;

            Game.Player.Revive(new Vector3(spawnLocation.X, spawnLocation.Y, spawnLocation.Z), heading);

            await BaseScript.Delay(1000);
            Game.PlayerPed.IsInvincible = false;
            await ScreenInterface.FadeIn(3000);
        }
    }
}
