using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;
using UnityEngine;

namespace SmoothTalker.Patches
{
    internal static class EnemySpottedHandler
    {
        internal static void Run(object instance)
        {
            if (!Settings.EnemySpottedEnabled.Value)
                return;

            if (Settings.EnemySpottedOutOfCombatOnly.Value && Helpers.InCombat)
                return;

            if (SendPingPrefix.SuppressedThisFrame)
                return;

            try
            {
                if (!Helpers.TryRaycastFikaPing(instance, out Player player, out RaycastHit hit))
                    return;

                if (!Helpers.IsEnemyPlayerPing(hit.collider.gameObject, player))
                    return;

                Helpers.TryPlayVoiceline(player, Settings.EnemySpottedTrigger.Value, requireCombat: false);
            }
            catch
            {
            }
        }
    }

    internal sealed class ClientEnemySpottedPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod() =>
            AccessTools.Method(AccessTools.TypeByName("Fika.Core.Main.PacketHandlers.ClientPacketSender"), "SendPing")
            ?? throw new InvalidOperationException("ClientPacketSender.SendPing not found");

        [PatchPostfix]
        private static void Postfix(object __instance) => EnemySpottedHandler.Run(__instance);
    }

    internal sealed class ServerEnemySpottedPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod() =>
            AccessTools.Method(AccessTools.TypeByName("Fika.Core.Main.PacketHandlers.ServerPacketSender"), "SendPing")
            ?? throw new InvalidOperationException("ServerPacketSender.SendPing not found");

        [PatchPostfix]
        private static void Postfix(object __instance) => EnemySpottedHandler.Run(__instance);
    }
}
