using System.Reflection;
using Comfort.Common;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SmoothTalker.Patches
{
    internal sealed class DryShotPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
            => AccessTools.Method(typeof(Player.FirearmController), "DryShot");

        [PatchPostfix]
        private static void Postfix(Player.FirearmController __instance)
        {
            if (!SmoothTalkerConfig.OutOfAmmoEnabled.Value)
                return;

            Player player = PatchHelper.GetControllerPlayer(__instance);
            if (!PatchHelper.IsLocalPlayer(player))
                return;

            SmoothTalkerPlugin.Log("[SmoothTalker] Dry shot — out of ammo");
            SmoothTalkerPlugin.TryPlayVoiceline(player, SmoothTalkerConfig.OutOfAmmoTrigger.Value);
        }
    }
}
