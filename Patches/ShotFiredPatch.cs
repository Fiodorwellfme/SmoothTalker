using System.Reflection;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;
using UnityEngine;

namespace SmoothTalker.Patches
{
    // Tracks the last time the local player fired a shot.
    internal sealed class ShotFiredPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
            => AccessTools.Method(typeof(Player.FirearmController), "InitiateShot");

        [PatchPostfix]
        private static void Postfix(Player.FirearmController __instance)
        {
            Player player = Helpers.GetControllerPlayer(__instance);
            if (!Helpers.IsLocalPlayer(player))
                return;

            Helpers.MarkShotFired();
        }
    }
}
