using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SmoothTalker.Patches
{
    internal static class ReloadHelpers
    {
        public static void TryPlayReloadVoiceline(Player.ItemHandsController controller)
        {
            if (!Settings.ReloadEnabled.Value)
                return;

            Player player = Helpers.GetControllerPlayer(controller);
            if (!Helpers.IsLocalPlayer(player))
                return;

            Plugin.LogSource.LogInfo("[SmoothTalker] Reload started");
            Helpers.TryPlayVoiceline(player, Settings.ReloadTrigger.Value);
        }
    }

    internal sealed class ReloadPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(Player.FirearmController), nameof(Player.FirearmController.ReloadMag));
        }

        [PatchPostfix]
        private static void Postfix(Player.FirearmController __instance)
        {
            ReloadHelpers.TryPlayReloadVoiceline(__instance);
        }
    }

    internal sealed class FikaReloadPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            var firearmControllerType = AccessTools.TypeByName("Fika.Core.Main.ClientClasses.HandsControllers.FikaClientFirearmController");
            if (firearmControllerType == null)
                throw new InvalidOperationException("FikaClientFirearmController was not found.");

            return AccessTools.Method(firearmControllerType, nameof(Player.FirearmController.ReloadMag));
        }

        [PatchPostfix]
        private static void Postfix(Player.FirearmController __instance)
        {
            ReloadHelpers.TryPlayReloadVoiceline(__instance);
        }
    }
}
