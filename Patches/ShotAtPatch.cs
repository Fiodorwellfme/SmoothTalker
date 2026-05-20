using System.Reflection;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;
using UnityEngine;

namespace SmoothTalker.Patches
{
    internal sealed class ShotAtPatch : ModulePatch
    {
        private static float _nextAllowedTime;

        protected override MethodBase GetTargetMethod()
            => AccessTools.Method(typeof(FlyingBulletSoundPlayer), "method_5");

        [PatchPostfix]
        private static void Postfix(FlyingBulletSoundPlayer __instance)
        {
            if (!Settings.ShotAtEnabled.Value)
                return;

            if (Helpers.InCombat)
                return;

            Player player = AccessTools.FieldRefAccess<FlyingBulletSoundPlayer, Player>(__instance, "player_0");
            if (!Helpers.IsLocalPlayer(player))
                return;

            float cooldown = Settings.ShotAtCooldown.Value;
            if (Time.time < _nextAllowedTime)
            {
                _nextAllowedTime = Time.time + cooldown;
                return;
            }

            Helpers.TryPlayVoiceline(player, Settings.ShotAtTrigger.Value, requireCombat: false);
            _nextAllowedTime = Time.time + cooldown;
        }
    }
}
