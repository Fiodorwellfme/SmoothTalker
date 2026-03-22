using System.Reflection;
using Comfort.Common;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SmoothTalker.Patches
{
    internal sealed class GrenadeHighThrowPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
            => AccessTools.Method(typeof(Player.GrenadeHandsController), "HighThrow");

        [PatchPostfix]
        private static void Postfix(Player.GrenadeHandsController __instance)
        {
            GrenadeThrowHelper.OnThrow(__instance);
        }
    }

    internal sealed class GrenadeLowThrowPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
            => AccessTools.Method(typeof(Player.GrenadeHandsController), "LowThrow");

        [PatchPostfix]
        private static void Postfix(Player.GrenadeHandsController __instance)
        {
            GrenadeThrowHelper.OnThrow(__instance);
        }
    }

    internal static class GrenadeThrowHelper
    {
        public static void OnThrow(Player.GrenadeHandsController controller)
        {
            if (!SmoothTalkerConfig.GrenadeEnabled.Value)
                return;

            Player player = PatchHelper.GetControllerPlayer(controller);
            if (!PatchHelper.IsLocalPlayer(player))
                return;

            SmoothTalkerPlugin.Log("[SmoothTalker] Grenade thrown");
            SmoothTalkerPlugin.TryPlayVoiceline(player, SmoothTalkerConfig.GrenadeTrigger.Value, requireCombat: false);
        }
    }
}
