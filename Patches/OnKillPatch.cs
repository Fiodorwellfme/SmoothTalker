using System.Reflection;
using Comfort.Common;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SmoothTalker.Patches
{
    internal sealed class OnKillPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
            => AccessTools.Method(typeof(Player), "OnBeenKilledByAggressor");

        [PatchPostfix]
        private static void Postfix(Player __instance, IPlayer aggressor)
        {
            if (aggressor == null || !Singleton<GameWorld>.Instantiated)
                return;

            Player mainPlayer = Singleton<GameWorld>.Instance.MainPlayer;
            if (mainPlayer == null)
                return;

            // Only trigger when the local player is the one who got the kill
            if (aggressor.ProfileId != mainPlayer.ProfileId)
                return;

            // Don't play voiceline for killing yourself
            if (__instance.ProfileId == mainPlayer.ProfileId)
                return;

            bool isScav = __instance.Side == EPlayerSide.Savage;
            string killType = isScav ? "Scav" : "PMC";

            SmoothTalkerPlugin.Log(
                $"[SmoothTalker] {killType} kill: {__instance.Profile.Nickname} — playing " +
                (isScav ? SmoothTalkerConfig.ScavVoiceTrigger.Value : SmoothTalkerConfig.PmcVoiceTrigger.Value));

            SmoothTalkerPlugin.TryPlayKillVoiceline(isScav);
        }
    }
}
