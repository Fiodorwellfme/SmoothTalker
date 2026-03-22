using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SmoothTalker.Patches
{
    internal static class ReloadPatchHelper
    {
        public static void TryHandle(Player.ItemHandsController controller)
        {
            if (!SmoothTalkerConfig.ReloadEnabled.Value)
                return;

            Player player = PatchHelper.GetControllerPlayer(controller);
            if (!PatchHelper.IsLocalPlayer(player))
                return;

            SmoothTalkerPlugin.Log("[SmoothTalker] Reload started");
            SmoothTalkerPlugin.TryPlayVoiceline(player, SmoothTalkerConfig.ReloadTrigger.Value);
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
            ReloadPatchHelper.TryHandle(__instance);
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
            ReloadPatchHelper.TryHandle(__instance);
        }
    }
}
