using System.Collections;
using System;
using System.Reflection;
using Comfort.Common;
using EFT;
using HarmonyLib;
using UnityEngine;

namespace SmoothTalker.Patches
{
    internal static class Helpers
    {
        private static bool _killVoicelinePending;
        private static float _lastPlayTime;
        private static float _lastShotTime;
        private static float _fikaPingRange;
        private static int _fikaPingMask;

        internal static bool InCombat
            => Time.time - _lastShotTime <= Settings.CombatTimeout.Value;

        internal static void MarkShotFired()
        {
            _lastShotTime = Time.time;
        }

        internal static void Log(string msg)
        {
            Plugin.LogSource?.LogInfo(msg);
        }

        internal static void TryPlayKillVoiceline(bool isScavKill)
        {
            if (!Settings.Enabled.Value)
                return;

            if (Plugin.Instance == null)
                return;

            if (_killVoicelinePending)
                return;

            if (Settings.KillDelay.Value <= 0f)
            {
                TryPlayKillVoicelineImmediate(isScavKill);
                return;
            }

            _killVoicelinePending = true;
            Plugin.Instance.StartCoroutine(DelayedKillVoicelineCoroutine(isScavKill));
        }

        internal static void TryPlayVoiceline(Player player, EPhraseTrigger trigger, bool requireCombat = true)
        {
            if (!Settings.Enabled.Value)
                return;

            if (Settings.MultiplayerOnly.Value && !FikaDetection.IsMultiplayer())
                return;

            if (requireCombat && Settings.CombatOnly.Value && !InCombat)
                return;

            if (Time.time - _lastPlayTime < Settings.Cooldown.Value)
                return;

            if (player == null || player.Speaker == null)
                return;

            player.Speaker.Play(
                trigger,
                player.HealthStatus | ETagStatus.Combat,
                true,
                200);

            _lastPlayTime = Time.time;
        }

        internal static void PlayPingSuppressionVoiceline(Player player)
        {
            if (player == null || player.Speaker == null)
                return;

            player.Speaker.Play(
                Settings.PingSuppressionTrigger.Value,
                player.HealthStatus | ETagStatus.Combat,
                true,
                200);
        }

        private static void TryPlayKillVoicelineImmediate(bool isScavKill)
        {
            if (!Singleton<GameWorld>.Instantiated)
                return;

            Player mainPlayer = Singleton<GameWorld>.Instance.MainPlayer;
            if (mainPlayer == null)
                return;

            EPhraseTrigger trigger = isScavKill
                ? Settings.ScavVoiceTrigger.Value
                : Settings.PmcVoiceTrigger.Value;

            TryPlayVoiceline(mainPlayer, trigger, requireCombat: true);
        }

        private static IEnumerator DelayedKillVoicelineCoroutine(bool isScavKill)
        {
            yield return new WaitForSeconds(Settings.KillDelay.Value);

            _killVoicelinePending = false;
            TryPlayKillVoicelineImmediate(isScavKill);
        }
        public static Player GetControllerPlayer(Player.ItemHandsController controller)
        {
            return AccessTools.FieldRefAccess<Player.ItemHandsController, Player>(controller, "_player");
        }

        public static bool IsLocalPlayer(Player player)
        {
            if (player == null || !Singleton<GameWorld>.Instantiated)
                return false;

            Player mainPlayer = Singleton<GameWorld>.Instance.MainPlayer;
            return mainPlayer != null && player.ProfileId == mainPlayer.ProfileId;
        }

        internal static bool TryRaycastFikaPing(object instance, out Player player, out RaycastHit hit)
        {
            InitFikaPingRaycast();

            FieldInfo playerField = AccessTools.Field(instance.GetType(), "_player");
            player = playerField?.GetValue(instance) as Player;
            if (player == null || !player.HealthController.IsAlive)
            {
                hit = default;
                return false;
            }

            return Physics.Raycast(GetFikaPingSourceRay(player), out hit, _fikaPingRange, _fikaPingMask);
        }

        internal static bool IsEnemyPlayerPing(GameObject hitGameObject, Player localPlayer)
        {
            if (LayerMask.LayerToName(hitGameObject.layer) != "Player")
                return false;

            if (!hitGameObject.TryGetComponent(out Player pingedPlayer))
                return false;

            if (pingedPlayer.ProfileId == localPlayer.ProfileId)
                return false;

            return !FikaDetection.IsHumanPlayer(pingedPlayer.ProfileId);
        }

        private static void InitFikaPingRaycast()
        {
            if (_fikaPingRange > 0f && _fikaPingMask != 0)
                return;

            Type globalsType = AccessTools.TypeByName("Fika.Core.Main.Utils.FikaGlobals");
            _fikaPingRange = globalsType != null ? Convert.ToSingle(AccessTools.Field(globalsType, "PingRange")?.GetValue(null)) : 0f;
            _fikaPingMask = globalsType != null ? (int)(AccessTools.Field(globalsType, "PingMask")?.GetValue(null) ?? 0) : 0;

            if (_fikaPingRange <= 0f)
                _fikaPingRange = 1000f;
            if (_fikaPingMask == 0)
                _fikaPingMask = Physics.DefaultRaycastLayers;
        }

        private static Ray GetFikaPingSourceRay(Player player)
        {
            if (player.HandsController is Player.FirearmController controller && controller.IsAiming)
                return new Ray(controller.FireportPosition, controller.WeaponDirection);

            Transform cameraPosition = player.CameraPosition;
            return new Ray(cameraPosition.position + cameraPosition.forward / 2f, player.LookDirection);
        }
    }
}
