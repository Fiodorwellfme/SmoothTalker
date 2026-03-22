using Comfort.Common;
using EFT;
using HarmonyLib;

namespace SmoothTalker.Patches
{
    internal static class PatchHelper
    {
        /// <summary>
        /// Extracts the owning Player from any Player.ItemHandsController subclass
        /// (FirearmController, GrenadeHandsController, etc.) via the _player field.
        /// </summary>
        public static Player GetControllerPlayer(Player.ItemHandsController controller)
        {
            return AccessTools.FieldRefAccess<Player.ItemHandsController, Player>(controller, "_player");
        }

        public static bool IsLocalPlayer(Player player)
        {
            if (player == null || !Singleton<GameWorld>.Instantiated)
                return false;
            Player mainPlayer = Singleton<GameWorld>.Instance.MainPlayer;
            return mainPlayer != null && player.ProfileId == mainPlayer.ProfileId;
        }
    }
}
