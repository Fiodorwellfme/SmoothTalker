using System;
using System.Collections;
using System.Reflection;
using HarmonyLib;

namespace SmoothTalker
{
    internal static class FikaDetection
    {
        private static bool _resolved;
        private static MethodInfo _tryGetCoopHandlerMethod;
        private static PropertyInfo _humanPlayersProp;
        private static PropertyInfo _isYourPlayerProp;
        private static PropertyInfo _profileIdProp;
        private static PropertyInfo _healthControllerProp;
        private static PropertyInfo _isAliveProp;

        internal static bool FikaInstalled { get; private set; }

        internal static void Init()
        {
            if (_resolved)
                return;

            _resolved = true;

            try
            {
                var coopHandlerType = AccessTools.TypeByName("Fika.Core.Main.Components.CoopHandler");
                if (coopHandlerType == null)
                {
                    SmoothTalkerPlugin.Log("[SmoothTalker] Fika not detected — multiplayer-only filter will be unavailable.");
                    return;
                }

                _tryGetCoopHandlerMethod = coopHandlerType.GetMethod(
                    "TryGetCoopHandler",
                    BindingFlags.Public | BindingFlags.Static);
                _humanPlayersProp = coopHandlerType.GetProperty(
                    "HumanPlayers",
                    BindingFlags.Public | BindingFlags.Instance);

                var playerType = AccessTools.TypeByName("EFT.Player");
                if (playerType == null)
                {
                    SmoothTalkerPlugin.Log("[SmoothTalker] EFT.Player not found — multiplayer-only filter will be unavailable.");
                    return;
                }

                _isYourPlayerProp = playerType.GetProperty("IsYourPlayer", BindingFlags.Public | BindingFlags.Instance);
                _profileIdProp = playerType.GetProperty("ProfileId", BindingFlags.Public | BindingFlags.Instance);
                _healthControllerProp = playerType.GetProperty("HealthController", BindingFlags.Public | BindingFlags.Instance);

                var healthControllerType = _healthControllerProp?.PropertyType;
                _isAliveProp = healthControllerType?.GetProperty("IsAlive", BindingFlags.Public | BindingFlags.Instance);

                if (_tryGetCoopHandlerMethod == null
                    || _humanPlayersProp == null
                    || _isYourPlayerProp == null
                    || _profileIdProp == null
                    || _healthControllerProp == null
                    || _isAliveProp == null)
                {
                    SmoothTalkerPlugin.Log("[SmoothTalker] Required Fika player-state members not found — multiplayer-only filter will be unavailable.");
                    return;
                }

                FikaInstalled = true;
                SmoothTalkerPlugin.Log("[SmoothTalker] Fika detected — multiplayer-only filter available.");
            }
            catch (Exception ex)
            {
                SmoothTalkerPlugin.Log($"[SmoothTalker] Error resolving Fika: {ex.Message}");
            }
        }

        internal static bool IsMultiplayer()
        {
            if (!FikaInstalled)
                return false;

            try
            {
                object[] args = [null];
                var foundHandler = (bool)_tryGetCoopHandlerMethod.Invoke(null, args);
                if (!foundHandler || args[0] == null)
                    return false;

                if (_humanPlayersProp.GetValue(args[0]) is not IList humanPlayers)
                    return false;

                for (var i = 0; i < humanPlayers.Count; i++)
                {
                    var player = humanPlayers[i];
                    if (player == null)
                        continue;

                    var isYourPlayer = (bool)_isYourPlayerProp.GetValue(player);
                    if (isYourPlayer)
                        continue;

                    var healthController = _healthControllerProp.GetValue(player);
                    if (healthController == null)
                        continue;

                    if ((bool)_isAliveProp.GetValue(healthController))
                        return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                SmoothTalkerPlugin.Log($"[SmoothTalker] Error checking Fika multiplayer state: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Returns true when <paramref name="profileId"/> belongs to any human player
        /// in the current Fika raid (including the local player). All humans in Fika
        /// are teammates, so this doubles as a teammate filter.
        /// Always returns false when Fika is not installed.
        /// </summary>
        internal static bool IsHumanPlayer(string profileId)
        {
            if (!FikaInstalled) return false;

            try
            {
                object[] args = [null];
                if (!(bool)_tryGetCoopHandlerMethod.Invoke(null, args) || args[0] == null)
                    return false;

                if (_humanPlayersProp.GetValue(args[0]) is not IList humanPlayers)
                    return false;

                for (var i = 0; i < humanPlayers.Count; i++)
                {
                    var player = humanPlayers[i];
                    if (player != null && (string)_profileIdProp.GetValue(player) == profileId)
                        return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
