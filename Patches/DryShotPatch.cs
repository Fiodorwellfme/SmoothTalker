using System.Reflection;
using Comfort.Common;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;
using EFT.InventoryLogic;

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
            if (__instance.Item is not Weapon weapon)
                return;
            if (weapon.MalfState.State != Weapon.EMalfunctionState.None)
                return;

            Player player = PatchHelper.GetControllerPlayer(__instance);
            if (!PatchHelper.IsLocalPlayer(player))
                return;

            SmoothTalkerPlugin.Log("[SmoothTalker] Dry shot — out of ammo");
            SmoothTalkerPlugin.TryPlayVoiceline(player, SmoothTalkerConfig.OutOfAmmoTrigger.Value);
        }
    }
}
