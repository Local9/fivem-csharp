using CitizenFX.Core.UI;

namespace ProjectName.Client.UI
{
    internal class ScreenInterface
    {
        /// <summary>
        /// Fades the screen out.
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        internal static async Task FadeOut(int ms = 1000)
        {
            Screen.Fading.FadeOut(ms);
            while (IsScreenFadingOut())
            {
                await BaseScript.Delay(10);
                if (Screen.Fading.IsFadedOut) break;
            }
        }

        /// <summary>
        /// Fades the screen in.
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        internal static async Task FadeIn(int ms = 1000)
        {
            Screen.Fading.FadeIn(ms);
            while (IsScreenFadingIn())
            {
                await BaseScript.Delay(10);
                if (Screen.Fading.IsFadedIn) break;
            }
        }
    }
}
