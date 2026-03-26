using System;
using System.Reflection;
using Comfort.Common;
using EFT;
using HarmonyLib;
using UnityEngine;

namespace SmoothTalker.Patches
{
    internal static class ShotAtHandler
    {
        private static bool _subscribed;
        private static MethodInfo _calculateNormal;
        private static FieldInfo _shooterField;
        private static bool _shooterFieldSearched;

        public static void TrySubscribe()
        {
            if (_subscribed) return;

            try
            {
                _calculateNormal = AccessTools.Method(typeof(GClass897), "CalculateNormalFromPoint");
                if (_calculateNormal == null)
                {
                    SmoothTalkerPlugin.Log("[ShotAt] GClass897.CalculateNormalFromPoint not found.");
                    return;
                }

                EventInfo ev = typeof(GClass897).GetEvent(
                    "OnShoot", BindingFlags.Public | BindingFlags.Static);
                if (ev == null)
                {
                    SmoothTalkerPlugin.Log("[ShotAt] GClass897.OnShoot event not found.");
                    return;
                }

                MethodInfo mi = typeof(ShotAtHandler).GetMethod(
                    nameof(HandleShot), BindingFlags.Static | BindingFlags.NonPublic);
                Delegate del = Delegate.CreateDelegate(ev.EventHandlerType, mi);
                ev.AddEventHandler(null, del);

                _subscribed = true;
                SmoothTalkerPlugin.Log("[ShotAt] Subscribed to GClass897.OnShoot.");
            }
            catch (Exception ex)
            {
                SmoothTalkerPlugin.Log($"[ShotAt] Setup failed: {ex.Message}");
            }
        }

        private static void HandleShot(GClass897 shot)
        {
            try
            {
                if (!SmoothTalkerConfig.ShotAtEnabled.Value) return;
                if (!Singleton<GameWorld>.Instantiated) return;

                Player mainPlayer = Singleton<GameWorld>.Instance.MainPlayer;
                if (mainPlayer == null) return;

                // Skip own shots
                IPlayer shooter = GetShooter(shot);
                if (shooter != null && shooter.ProfileId == mainPlayer.ProfileId)
                    return;

                // Perpendicular distance from the local player to the bullet's path line.
                // Zero vector = bullet not passing near the player (outside trajectory segment).
                var normalObj = _calculateNormal.Invoke(shot, new object[] { mainPlayer.Position });
                if (normalObj is not Vector3 normal) return;
                if (normal == Vector3.zero) return;
                if (normal.magnitude > SmoothTalkerConfig.ShotAtRadius.Value) return;

                SmoothTalkerPlugin.Log(
                    $"[ShotAt] Bullet passed {normal.magnitude:F2}m from player — triggering voiceline.");
                SmoothTalkerPlugin.TryPlayVoiceline(
                    mainPlayer, SmoothTalkerConfig.ShotAtTrigger.Value, requireCombat: false);
            }
            catch { /* swallow to avoid disrupting the game loop */ }
        }

        private static IPlayer GetShooter(GClass897 shot)
        {
            if (!_shooterFieldSearched)
            {
                _shooterFieldSearched = true;
                foreach (string name in new[] { "Player", "iPlayer", "Shooter", "player", "_player", "ShootingPlayer" })
                {
                    _shooterField = typeof(GClass897).GetField(
                        name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (_shooterField != null) break;
                }
            }
            return _shooterField?.GetValue(shot) as IPlayer;
        }
    }
}
