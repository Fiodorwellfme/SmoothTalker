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
            if (!Settings.OutOfAmmoEnabled.Value)
                return;
            if (__instance.Item is not Weapon weapon)
                return;
            if (weapon.MalfState.State != Weapon.EMalfunctionState.None)
                return;

            Player player = Helpers.GetControllerPlayer(__instance);
            if (!Helpers.IsLocalPlayer(player))
                return;

            Plugin.LogSource.LogInfo("[SmoothTalker] Dry shot — out of ammo");
            Helpers.TryPlayVoiceline(player, Settings.OutOfAmmoTrigger.Value);
        }
    }
}
