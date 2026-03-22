using System.Reflection;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;
using UnityEngine;

namespace SmoothTalker.Patches
{
    /// <summary>
    /// Tracks the last time the local player fired a shot.
    /// Used by the combat-only voiceline filter.
    /// </summary>
    internal sealed class ShotFiredPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
            => AccessTools.Method(typeof(Player.FirearmController), "InitiateShot");

        [PatchPostfix]
        private static void Postfix(Player.FirearmController __instance)
        {
            Player player = PatchHelper.GetControllerPlayer(__instance);
            if (!PatchHelper.IsLocalPlayer(player))
                return;

            SmoothTalkerPlugin.LastShotTime = Time.time;
        }
    }
}
