using BepInEx.Configuration;
using EFT;
using System.Collections.Generic;

namespace SmoothTalker
{
    internal static class Settings
    {
        public static ConfigFile Config;
        public static List<ConfigEntryBase> ConfigEntries = new List<ConfigEntryBase>();

        public static ConfigEntry<bool> Enabled;
        public static ConfigEntry<float> Cooldown;
        public static ConfigEntry<bool> CombatOnly;
        public static ConfigEntry<float> CombatTimeout;
        public static ConfigEntry<bool> MultiplayerOnly;

        public static ConfigEntry<float> KillDelay;
        public static ConfigEntry<EPhraseTrigger> PmcVoiceTrigger;
        public static ConfigEntry<EPhraseTrigger> ScavVoiceTrigger;

        public static ConfigEntry<bool> ReloadEnabled;
        public static ConfigEntry<EPhraseTrigger> ReloadTrigger;

        public static ConfigEntry<bool> OutOfAmmoEnabled;
        public static ConfigEntry<EPhraseTrigger> OutOfAmmoTrigger;

        public static ConfigEntry<bool> GrenadeEnabled;
        public static ConfigEntry<EPhraseTrigger> GrenadeTrigger;

        public static ConfigEntry<bool> ShotAtEnabled;
        public static ConfigEntry<float> ShotAtCooldown;
        public static ConfigEntry<EPhraseTrigger> ShotAtTrigger;

        public static ConfigEntry<bool> EnemySpottedEnabled;
        public static ConfigEntry<bool> EnemySpottedOutOfCombatOnly;
        public static ConfigEntry<EPhraseTrigger> EnemySpottedTrigger;

        public static ConfigEntry<bool> PingSuppressionEnabled;
        public static ConfigEntry<float> PingSuppressionRadius;
        public static ConfigEntry<float> PingSuppressionExpiry;
        public static ConfigEntry<EPhraseTrigger> PingSuppressionTrigger;

        public static void Init(ConfigFile config)
        {
            Config = config;
            ConfigEntries.Clear();

            ConfigEntries.Add(Enabled = config.Bind("1. General", "Enabled", true,
                new ConfigDescription(
                    "Master toggle for all voicelines.",
                    null,
                    new global::ConfigurationManagerAttributes { IsAdvanced = false })));

            ConfigEntries.Add(Cooldown = config.Bind("1. General", "Cooldown (seconds)", 5f,
                new ConfigDescription(
                    "Minimum seconds between any voiceline to avoid spam.",
                    new AcceptableValueRange<float>(0f, 30f),
                    new global::ConfigurationManagerAttributes { IsAdvanced = false })));

            ConfigEntries.Add(CombatOnly = config.Bind("1. General", "Combat Only", true,
                new ConfigDescription(
                    "Only play voicelines (kills, reload, out-of-ammo) when in combat.\n" +
                    "Combat = player fired a shot within the timeout window.\n" +
                    "Grenade throws and shot-at voicelines always play regardless of this setting.",
                    null,
                    new global::ConfigurationManagerAttributes { IsAdvanced = false })));

            ConfigEntries.Add(CombatTimeout = config.Bind("1. General", "Combat Timeout (seconds)", 2f,
                new ConfigDescription(
                    "How many seconds after the last shot until the player is considered out of combat.",
                    new AcceptableValueRange<float>(1f, 60f),
                    new global::ConfigurationManagerAttributes { IsAdvanced = false })));

            ConfigEntries.Add(MultiplayerOnly = config.Bind("1. General", "Not a schizophrenic", false,
                new ConfigDescription(
                    "Only play voicelines when at least one other human player is alive.\n" +
                    "Requires Fika to be installed. When Fika is not present this setting has no effect.",
                    null,
                    new global::ConfigurationManagerAttributes { IsAdvanced = false })));

            ConfigEntries.Add(KillDelay = config.Bind("2. Kills", "Kill Delay (seconds)", 0f,
                new ConfigDescription(
                    "Delay before playing a kill voiceline.",
                    new AcceptableValueRange<float>(0f, 30f),
                    new global::ConfigurationManagerAttributes { IsAdvanced = false })));

            ConfigEntries.Add(PmcVoiceTrigger = config.Bind("2. Kills", "PMC Kill Voice", EPhraseTrigger.EnemyDown,
                new ConfigDescription(
                    "Voice line when killing a PMC (USEC/BEAR).",
                    null,
                    new global::ConfigurationManagerAttributes { IsAdvanced = false })));

            ConfigEntries.Add(ScavVoiceTrigger = config.Bind("2. Kills", "Scav Kill Voice", EPhraseTrigger.ScavDown,
                new ConfigDescription(
                    "Voice line when killing a Scav.",
                    null,
                    new global::ConfigurationManagerAttributes { IsAdvanced = false })));

            ConfigEntries.Add(ReloadEnabled = config.Bind("3. Reload", "Enabled", true,
                new ConfigDescription(
                    "Play a voice line when reloading.",
                    null,
                    new global::ConfigurationManagerAttributes { IsAdvanced = false })));

            ConfigEntries.Add(ReloadTrigger = config.Bind("3. Reload", "Voice Trigger", EPhraseTrigger.OnWeaponReload,
                new ConfigDescription(
                    "Voice line to play on reload.",
                    null,
                    new global::ConfigurationManagerAttributes { IsAdvanced = false })));

            ConfigEntries.Add(OutOfAmmoEnabled = config.Bind("4. Out of Ammo", "Enabled", false,
                new ConfigDescription(
                    "Play a voice line on dry fire (empty chamber).",
                    null,
                    new global::ConfigurationManagerAttributes { IsAdvanced = false })));

            ConfigEntries.Add(OutOfAmmoTrigger = config.Bind("4. Out of Ammo", "Voice Trigger", EPhraseTrigger.OnOutOfAmmo,
                new ConfigDescription(
                    "Voice line to play when out of ammo.",
                    null,
                    new global::ConfigurationManagerAttributes { IsAdvanced = false })));

            ConfigEntries.Add(GrenadeEnabled = config.Bind("5. Grenade", "Enabled", true,
                new ConfigDescription(
                    "Play a voice line when throwing a grenade.",
                    null,
                    new global::ConfigurationManagerAttributes { IsAdvanced = false })));

            ConfigEntries.Add(GrenadeTrigger = config.Bind("5. Grenade", "Voice Trigger", EPhraseTrigger.OnGrenade,
                new ConfigDescription(
                    "Voice line to play on grenade throw.",
                    null,
                    new global::ConfigurationManagerAttributes { IsAdvanced = false })));

            ConfigEntries.Add(ShotAtEnabled = config.Bind("6. Shot At", "Enabled", true,
                new ConfigDescription(
                    "Play a voice line when a bullet passes close enough to trigger the vanilla flying-bullet vignette effect.",
                    null,
                    new global::ConfigurationManagerAttributes { IsAdvanced = false })));

            ConfigEntries.Add(ShotAtCooldown = config.Bind("6. Shot At", "Cooldown (seconds)", 20f,
                new ConfigDescription(
                    "Seconds before another shot-at voice line can play. New shot-at triggers during cooldown restart this timer.",
                    new AcceptableValueRange<float>(0f, 120f),
                    new global::ConfigurationManagerAttributes { IsAdvanced = false })));

            ConfigEntries.Add(ShotAtTrigger = config.Bind("6. Shot At", "Voice Trigger", EPhraseTrigger.Spreadout,
                new ConfigDescription(
                    "Voice line to play when being shot at.",
                    null,
                    new global::ConfigurationManagerAttributes { IsAdvanced = false })));

            ConfigEntries.Add(EnemySpottedEnabled = config.Bind("7. Enemy Spotted", "Enabled", true,
                new ConfigDescription(
                    "Play a voice line when your Fika ping targets an enemy player.",
                    null,
                    new global::ConfigurationManagerAttributes { IsAdvanced = false })));

            ConfigEntries.Add(EnemySpottedOutOfCombatOnly = config.Bind("7. Enemy Spotted", "Only Play While Out Of Combat", true,
                new ConfigDescription(
                    "Only play the enemy spotted voice line when you are not already in combat.",
                    null,
                    new global::ConfigurationManagerAttributes { IsAdvanced = false })));

            ConfigEntries.Add(EnemySpottedTrigger = config.Bind("7. Enemy Spotted", "Voice Trigger", EPhraseTrigger.OnRepeatedContact,
                new ConfigDescription(
                    "Voice line to play when pinging an enemy player.",
                    null,
                    new global::ConfigurationManagerAttributes { IsAdvanced = false })));

            ConfigEntries.Add(PingSuppressionEnabled = config.Bind("8. Confirmation", "Enabled", true,
                new ConfigDescription(
                    "When enabled, pinging near an existing teammate ping plays a confirmation voice line\n" +
                    "and suppresses both the outbound PingPacket and local visual ping.\n" +
                    "Requires Fika to be installed.",
                    null,
                    new global::ConfigurationManagerAttributes { IsAdvanced = false })));

            ConfigEntries.Add(PingSuppressionRadius = config.Bind("8. Confirmation", "Suppression Radius (meters)", 5f,
                new ConfigDescription(
                    "How close (in metres) your ping target must be to an existing teammate ping\n" +
                    "for suppression to kick in.",
                    new AcceptableValueRange<float>(0.5f, 30f),
                    new global::ConfigurationManagerAttributes { IsAdvanced = false })));

            ConfigEntries.Add(PingSuppressionExpiry = config.Bind("8. Confirmation", "Suppression Delay (seconds)", 5f,
                new ConfigDescription(
                    "How long after a friendly ping you will confirm instead of placing your own ping.",
                    new AcceptableValueRange<float>(0.5f, 30f),
                    new global::ConfigurationManagerAttributes { IsAdvanced = false })));

            ConfigEntries.Add(PingSuppressionTrigger = config.Bind("8. Confirmation", "Confirmation Voice", EPhraseTrigger.Roger,
                new ConfigDescription(
                    "Voice line played on the local player when a ping is suppressed because a\n" +
                    "teammate already marked that location.",
                    null,
                    new global::ConfigurationManagerAttributes { IsAdvanced = false })));

            RecalcOrder();
        }

        private static void RecalcOrder()
        {
            int settingOrder = ConfigEntries.Count;
            foreach (ConfigEntryBase entry in ConfigEntries)
            {
                global::ConfigurationManagerAttributes attributes = entry.Description.Tags[0] as global::ConfigurationManagerAttributes;
                if (attributes != null)
                    attributes.Order = settingOrder;

                settingOrder--;
            }
        }
    }
}

