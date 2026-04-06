using System;
using System.Collections;
using BepInEx;
using Comfort.Common;
using EFT;
using UnityEngine;

namespace SmoothTalker
{
    [BepInPlugin("com.fiodor.smoothtalker", "SmoothTalker", "1.1.2")]
    public sealed class SmoothTalkerPlugin : BaseUnityPlugin
    {
        internal static SmoothTalkerPlugin Instance;

        internal static float LastPlayTime;
        internal static float LastShotTime;

        private bool _killVoicelinePending;

        private void Awake()
        {
            Instance = this;

            SmoothTalkerConfig.Init(Config);

            FikaDetection.Init();

            EnablePatch<Patches.ShotFiredPatch>();
            EnablePatch<Patches.OnKillPatch>();
            EnablePatch<Patches.ReloadPatch>();
            if (FikaDetection.FikaInstalled)
            {
                EnablePatch<Patches.FikaReloadPatch>();
                EnablePatch<Patches.ReceivePingPatch>();
                EnablePatch<Patches.ClientSendPingPatch>();
                EnablePatch<Patches.ServerSendPingPatch>();
            }
            EnablePatch<Patches.DryShotPatch>();
            EnablePatch<Patches.GrenadeHighThrowPatch>();
            EnablePatch<Patches.GrenadeLowThrowPatch>();


            Logger.LogInfo("SmoothTalker v1.1.2 loaded.");
        }

        private static void EnablePatch<T>() where T : SPT.Reflection.Patching.ModulePatch, new()
        {
            try
            {
                new T().Enable();
            }
            catch (Exception ex)
            {
                Instance?.Logger.LogError($"Failed to enable {typeof(T).Name}: {ex.Message}");
            }
        }

        internal static bool InCombat
            => Time.time - LastShotTime <= SmoothTalkerConfig.CombatTimeout.Value;

        internal static void TryPlayKillVoiceline(bool isScavKill)
        {
            if (!SmoothTalkerConfig.Enabled.Value)
                return;

            if (Instance == null)
                return;

            if (Instance._killVoicelinePending)
                return;

            if (SmoothTalkerConfig.KillDelay.Value <= 0f)
            {
                Instance.TryPlayKillVoicelineImmediate(isScavKill);
                return;
            }

            Instance._killVoicelinePending = true;
            Instance.StartCoroutine(Instance.DelayedKillVoicelineCoroutine(isScavKill));
        }

        internal static void TryPlayVoiceline(Player player, EPhraseTrigger trigger, bool requireCombat = true)
        {
            if (!SmoothTalkerConfig.Enabled.Value)
                return;

            if (SmoothTalkerConfig.MultiplayerOnly.Value && !FikaDetection.IsMultiplayer())
                return;

            if (requireCombat && SmoothTalkerConfig.CombatOnly.Value && !InCombat)
                return;

            if (Time.time - LastPlayTime < SmoothTalkerConfig.Cooldown.Value)
                return;

            if (player == null || player.Speaker == null)
                return;

            player.Speaker.Play(
                trigger,
                player.HealthStatus | ETagStatus.Combat,
                true,
                null);

            LastPlayTime = Time.time;
        }

        internal static void Log(string msg)
        {
            Instance?.Logger.LogInfo(msg);
        }

        private void TryPlayKillVoicelineImmediate(bool isScavKill)
        {
            if (!Singleton<GameWorld>.Instantiated)
                return;

            Player mainPlayer = Singleton<GameWorld>.Instance.MainPlayer;
            if (mainPlayer == null)
                return;

            EPhraseTrigger trigger = isScavKill
                ? SmoothTalkerConfig.ScavVoiceTrigger.Value
                : SmoothTalkerConfig.PmcVoiceTrigger.Value;

            TryPlayVoiceline(mainPlayer, trigger, requireCombat: true);
        }

        private IEnumerator DelayedKillVoicelineCoroutine(bool isScavKill)
        {
            yield return new WaitForSeconds(SmoothTalkerConfig.KillDelay.Value);

            _killVoicelinePending = false;
            TryPlayKillVoicelineImmediate(isScavKill);
        }
    }
}
