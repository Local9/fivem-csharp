using CitizenFX.Core.Native;

namespace ProjectName.Client.Extension
{
    internal static class EntityExtension
    {
        /// <summary>
        /// Fade out the entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="slow"></param>
        /// <returns></returns>
        public async static Task FadeOut(this Entity entity, bool slow = true)
        {
            await Fade(entity, false, slow);
        }

        /// <summary>
        /// Fade in the entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="slow"></param>
        /// <returns></returns>
        public async static Task FadeIn(this Entity entity, bool slow = true)
        {
            await Fade(entity, true, slow);
        }

        /// <summary>
        /// Fade control for the entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="fadeIn"></param>
        /// <param name="slow"></param>
        /// <param name="fadeVehicle"></param>
        /// <returns></returns>
        public async static Task Fade(this Entity entity, bool fadeIn, bool slow = true, bool fadeVehicle = false)
        {
            Vehicle vehicle = null;
            Ped ped = null;

            if (entity is Ped)
                ped = (Ped)entity;

            if (ped?.IsInVehicle() == true)
                vehicle = ped.CurrentVehicle;

            if (fadeIn)
            {
                Function.Call((Hash)0x1F4ED342ACEFE62D, entity.Handle, fadeIn, slow);

                if (vehicle is not null && fadeVehicle)
                    Function.Call((Hash)0x1F4ED342ACEFE62D, vehicle.Handle, fadeIn, slow);
            }
            else
            {
                API.NetworkFadeOutEntity(entity.Handle, false, slow);

                if (vehicle is not null && fadeVehicle)
                    API.NetworkFadeOutEntity(vehicle.Handle, false, slow);
            }

            while (API.NetworkIsEntityFading(entity.Handle))
            {
                await BaseScript.Delay(10);
            }
        }
    }
}
