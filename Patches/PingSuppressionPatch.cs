using System;
using System.Collections.Generic;
using System.Reflection;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;
using UnityEngine;

namespace SmoothTalker.Patches
{
    // Tracks active teammate ping positions with a time-based expiry

    internal static class PingRegistry
    {
        private static readonly List<(Vector3 pos, float exp)> _pings = new();

        public static void Add(Vector3 pos) => _pings.Add((pos, Time.time + Settings.PingSuppressionExpiry.Value));

        public static bool HasNearby(Vector3 point, float radius)
        {
            float now = Time.time, sqr = radius * radius;
            bool found = false;
            for (int i = _pings.Count - 1; i >= 0; i--)
            {
                if (_pings[i].exp <= now) { _pings.RemoveAt(i); continue; }
                if ((point - _pings[i].pos).sqrMagnitude <= sqr) found = true;
            }
            return found;
        }
    }

    internal static class SendPingPrefix
    {
        private static FieldInfo _lastPingTimeFi;
        private static int _suppressedFrame = -1;

        internal static bool SuppressedThisFrame => _suppressedFrame == Time.frameCount;

        private static void Init(object instance)
        {
            if (_lastPingTimeFi != null) return;
            _lastPingTimeFi = AccessTools.Field(instance.GetType(), "_lastPingTime");
        }

        public static bool Run(object instance)
        {
            if (!Settings.PingSuppressionEnabled.Value)
                return true;

            try
            {
                Init(instance);

                if (!Helpers.TryRaycastFikaPing(instance, out Player player, out RaycastHit hit))
                    return true;

                bool suppressPing = Settings.PingSuppressionEnabled.Value
                    && PingRegistry.HasNearby(hit.point, Settings.PingSuppressionRadius.Value);

                if (suppressPing)
                {
                    _suppressedFrame = Time.frameCount;
                    _lastPingTimeFi?.SetValue(instance, DateTime.Now);
                    Helpers.PlayPingSuppressionVoiceline(player);
                    return false;
                }

                return true;
            }
            catch { return true; }
        }
    }

    internal sealed class ReceivePingPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod() =>
            AccessTools.Method(AccessTools.TypeByName("Fika.Core.Main.Factories.PingFactory"), "ReceivePing")
            ?? throw new InvalidOperationException("PingFactory.ReceivePing not found");

        [PatchPostfix]
        static void Postfix(Vector3 location) => PingRegistry.Add(location);
    }

    /// <summary>Suppresses the outbound ping + local visual on ClientPacketSender.</summary>
    internal sealed class ClientSendPingPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod() =>
            AccessTools.Method(AccessTools.TypeByName("Fika.Core.Main.PacketHandlers.ClientPacketSender"), "SendPing")
            ?? throw new InvalidOperationException("ClientPacketSender.SendPing not found");

        [PatchPrefix]
        static bool Prefix(object __instance) => SendPingPrefix.Run(__instance);
    }

    /// <summary>Suppresses the outbound ping + local visual on ServerPacketSender.</summary>
    internal sealed class ServerSendPingPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod() =>
            AccessTools.Method(AccessTools.TypeByName("Fika.Core.Main.PacketHandlers.ServerPacketSender"), "SendPing")
            ?? throw new InvalidOperationException("ServerPacketSender.SendPing not found");

        [PatchPrefix]
        static bool Prefix(object __instance) => SendPingPrefix.Run(__instance);
    }
}
