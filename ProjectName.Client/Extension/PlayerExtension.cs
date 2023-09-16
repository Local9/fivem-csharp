using CitizenFX.Core.Native;

namespace ProjectName.Client.Extension
{
    internal static class PlayerExtension
    {
        /// <summary>
        /// Player Revive
        /// </summary>
        /// <param name="player"></param>
        /// <param name="position"></param>
        /// <param name="heading"></param>
        public static async void Revive(this Player player, Vector3 position, float heading = 0f)
        {
            Main.Logger.Debug($"Revive {player.Name} {position} {heading}");

            await player.Character.FadeOut();

            player.Character.Task.ClearAllImmediately();
            Game.Player.WantedLevel = 0;
            player.Character.IsVisible = true;
            player.Character.Health = player.Character.MaxHealth;
            player.Character.Armor = 0;
            player.Character.ClearBloodDamage();
            player.Character.ClearLastWeaponDamage();

            StopEntityFire(player.Character.Handle);
            SetPedStealthMovement(player.Character.Handle, false, "0");

            API.NetworkResurrectLocalPlayer(position.X, position.Y, position.Z, heading, false, false);
            player.Character.IsPositionFrozen = true;
            PlaceObjectOnGroundProperly(player.Character.Handle);
            player.Character.IsPositionFrozen = false;
            await player.Character.FadeIn();
        }
    }
}
