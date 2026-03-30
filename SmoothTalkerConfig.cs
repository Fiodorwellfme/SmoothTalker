using BepInEx.Configuration;
using EFT;

namespace SmoothTalker
{
    internal static class SmoothTalkerConfig
    {
        // General
        public static ConfigEntry<bool> Enabled { get; private set; }
        public static ConfigEntry<float> Cooldown { get; private set; }
        public static ConfigEntry<bool> CombatOnly { get; private set; }
        public static ConfigEntry<float> CombatTimeout { get; private set; }
        public static ConfigEntry<bool> MultiplayerOnly { get; private set; }

        // Kills
        public static ConfigEntry<float> KillDelay { get; private set; }
        public static ConfigEntry<EPhraseTrigger> PmcVoiceTrigger { get; private set; }
        public static ConfigEntry<EPhraseTrigger> ScavVoiceTrigger { get; private set; }

        // Reload
        public static ConfigEntry<bool> ReloadEnabled { get; private set; }
        public static ConfigEntry<EPhraseTrigger> ReloadTrigger { get; private set; }

        // Out of ammo
        public static ConfigEntry<bool> OutOfAmmoEnabled { get; private set; }
        public static ConfigEntry<EPhraseTrigger> OutOfAmmoTrigger { get; private set; }

        // Grenade
        public static ConfigEntry<bool> GrenadeEnabled { get; private set; }
        public static ConfigEntry<EPhraseTrigger> GrenadeTrigger { get; private set; }

        // Ping Suppression
        public static ConfigEntry<bool> PingSuppressionEnabled { get; private set; }
        public static ConfigEntry<float> PingSuppressionRadius { get; private set; }
        public static ConfigEntry<EPhraseTrigger> PingSuppressionTrigger { get; private set; }

        public static void Init(ConfigFile config)
        {
            const string general = "1. General";
            const string kills = "2. Kills";
            const string reload = "3. Reload";
            const string outOfAmmo = "4. Out of Ammo";
            const string grenade = "5. Grenade";
            const string pingSuppression = "6. Ping Suppression";

            Enabled = config.Bind(
                general,
                "Enabled",
                true,
                new ConfigDescription(
                    "Master toggle for all voicelines.",
                    null,
                    new global::ConfigurationManagerAttributes
                    {
                        Order = 5
                    }));

            Cooldown = config.Bind(
                general,
                "Cooldown (seconds)",
                5f,
                new ConfigDescription(
                    "Minimum seconds between any voiceline to avoid spam.",
                    new AcceptableValueRange<float>(0f, 30f),
                    new global::ConfigurationManagerAttributes
                    {
                        Order = 4
                    }));

            CombatOnly = config.Bind(
                general,
                "Combat Only",
                true,
                new ConfigDescription(
                    "Only play voicelines (kills, reload, out-of-ammo) when in combat.\n" +
                    "Combat = player fired a shot within the timeout window.\n" +
                    "Grenade throws always play regardless of this setting.",
                    null,
                    new global::ConfigurationManagerAttributes
                    {
                        Order = 3
                    }));

            CombatTimeout = config.Bind(
                general,
                "Combat Timeout (seconds)",
                2f,
                new ConfigDescription(
                    "How many seconds after the last shot until the player is considered out of combat.",
                    new AcceptableValueRange<float>(1f, 60f),
                    new global::ConfigurationManagerAttributes
                    {
                        Order = 2
                    }));

            MultiplayerOnly = config.Bind(
                general,
                "Not a schizophrenic",
                false,
                new ConfigDescription(
                    "Only play voicelines when at least one other human player is alive.\n" +
                    "Requires Fika to be installed. When Fika is not present this setting has no effect.",
                    null,
                    new global::ConfigurationManagerAttributes
                    {
                        Order = 1
                    }));

            KillDelay = config.Bind(
                kills,
                "Kill Delay (seconds)",
                0f,
                new ConfigDescription(
                    "Delay before playing a kill voiceline.",
                    new AcceptableValueRange<float>(0f, 30f),
                    new global::ConfigurationManagerAttributes
                    {
                        Order = 3
                    }));

            PmcVoiceTrigger = config.Bind(
                kills,
                "PMC Kill Voice",
                EPhraseTrigger.OnGoodWork,
                new ConfigDescription(
                    "Voice line when killing a PMC (USEC/BEAR).",
                    null,
                    new global::ConfigurationManagerAttributes
                    {
                        Order = 2
                    }));

            ScavVoiceTrigger = config.Bind(
                kills,
                "Scav Kill Voice",
                EPhraseTrigger.ScavDown,
                new ConfigDescription(
                    "Voice line when killing a Scav.",
                    null,
                    new global::ConfigurationManagerAttributes
                    {
                        Order = 1
                    }));

            ReloadEnabled = config.Bind(
                reload,
                "Enabled",
                true,
                new ConfigDescription(
                    "Play a voice line when reloading.",
                    null,
                    new global::ConfigurationManagerAttributes
                    {
                        Order = 2
                    }));

            ReloadTrigger = config.Bind(
                reload,
                "Voice Trigger",
                EPhraseTrigger.OnWeaponReload,
                new ConfigDescription(
                    "Voice line to play on reload.",
                    null,
                    new global::ConfigurationManagerAttributes
                    {
                        Order = 1
                    }));

            OutOfAmmoEnabled = config.Bind(
                outOfAmmo,
                "Enabled",
                false,
                new ConfigDescription(
                    "Play a voice line on dry fire (empty chamber).",
                    null,
                    new global::ConfigurationManagerAttributes
                    {
                        Order = 2
                    }));

            OutOfAmmoTrigger = config.Bind(
                outOfAmmo,
                "Voice Trigger",
                EPhraseTrigger.OnOutOfAmmo,
                new ConfigDescription(
                    "Voice line to play when out of ammo.",
                    null,
                    new global::ConfigurationManagerAttributes
                    {
                        Order = 1
                    }));

            GrenadeEnabled = config.Bind(
                grenade,
                "Enabled",
                true,
                new ConfigDescription(
                    "Play a voice line when throwing a grenade.",
                    null,
                    new global::ConfigurationManagerAttributes
                    {
                        Order = 2
                    }));

            GrenadeTrigger = config.Bind(
                grenade,
                "Voice Trigger",
                EPhraseTrigger.OnGrenade,
                new ConfigDescription(
                    "Voice line to play on grenade throw.",
                    null,
                    new global::ConfigurationManagerAttributes
                    {
                        Order = 1
                    }));

            PingSuppressionEnabled = config.Bind(
                pingSuppression,
                "Enabled",
                true,
                new ConfigDescription(
                    "When enabled, pinging near an existing teammate ping plays a confirmation voice line\n" +
                    "and suppresses both the outbound PingPacket and local visual ping.\n" +
                    "Requires Fika to be installed.",
                    null,
                    new global::ConfigurationManagerAttributes
                    {
                        Order = 3
                    }));

            PingSuppressionRadius = config.Bind(
                pingSuppression,
                "Suppression Radius (meters)",
                5f,
                new ConfigDescription(
                    "How close (in metres) your ping target must be to an existing teammate ping\n" +
                    "for suppression to kick in.",
                    new AcceptableValueRange<float>(0.5f, 30f),
                    new global::ConfigurationManagerAttributes
                    {
                        Order = 2
                    }));

            PingSuppressionTrigger = config.Bind(
                pingSuppression,
                "Confirmation Voice",
                EPhraseTrigger.OnGoodWork,
                new ConfigDescription(
                    "Voice line played on the local player when a ping is suppressed because a\n" +
                    "teammate already marked that location.",
                    null,
                    new global::ConfigurationManagerAttributes
                    {
                        Order = 1
                    }));
        }
    }
}