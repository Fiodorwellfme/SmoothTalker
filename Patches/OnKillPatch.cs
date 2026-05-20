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

            if (aggressor.ProfileId != mainPlayer.ProfileId)
                return;

            if (__instance.ProfileId == mainPlayer.ProfileId)
                return;

            WildSpawnType role = __instance.Profile.Info.Settings.Role;
            bool useScavLine = role == WildSpawnType.assault || role == WildSpawnType.marksman;
            string killType = useScavLine ? "Scav" : "PMC";

            Plugin.LogSource.LogInfo(
                $"[SmoothTalker] {killType} kill: {__instance.Profile.Nickname} — playing " +
                (useScavLine ? Settings.ScavVoiceTrigger.Value : Settings.PmcVoiceTrigger.Value));

            Helpers.TryPlayKillVoiceline(useScavLine);
        }
    }
}
