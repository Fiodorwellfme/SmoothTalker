using System;
using System.Collections.Generic;
using System.Reflection;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;
using UnityEngine;

namespace SmoothTalker.Patches
{
    /// Tracks active teammate ping positions with a time-based expiry hard set at 10 seconds, maybe make configurable 

    internal static class PingRegistry
    {
        private static readonly List<(Vector3 pos, float exp)> _pings = new();

        public static void Add(Vector3 pos) => _pings.Add((pos, Time.time + 10f));

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
        private static float _range;
        private static int _mask;
        private static FieldInfo _playerFi, _lastPingTimeFi;

        private static void Init(object instance)
        {
            if (_playerFi != null) return;

            var g = AccessTools.TypeByName("Fika.Core.Main.Utils.FikaGlobals");
            _range = g != null ? Convert.ToSingle(AccessTools.Field(g, "PingRange")?.GetValue(null)) : 0f;
            _mask  = g != null ? (int)(AccessTools.Field(g, "PingMask")?.GetValue(null) ?? 0) : 0;
            if (_range <= 0f) _range = 1000f;
            if (_mask  == 0)  _mask  = Physics.DefaultRaycastLayers;

            _playerFi      = AccessTools.Field(instance.GetType(), "_player");
            _lastPingTimeFi = AccessTools.Field(instance.GetType(), "_lastPingTime");
        }

        public static bool Run(object instance)
        {
            if (!SmoothTalkerConfig.PingSuppressionEnabled.Value) return true;

            try
            {
                Init(instance);

                var player = _playerFi?.GetValue(instance) as Player;
                if (player == null || !player.HealthController.IsAlive) return true;

                var cam = player.CameraPosition;
                if (!Physics.Raycast(new Ray(cam.position + cam.forward / 2f, player.LookDirection),
                        out var hit, _range, _mask))
                    return true;

                if (!PingRegistry.HasNearby(hit.point, SmoothTalkerConfig.PingSuppressionRadius.Value))
                    return true;

                _lastPingTimeFi?.SetValue(instance, DateTime.Now);
                player.Speaker.Play(SmoothTalkerConfig.PingSuppressionTrigger.Value,
                    player.HealthStatus | ETagStatus.Combat, true, null);
                return false;
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
