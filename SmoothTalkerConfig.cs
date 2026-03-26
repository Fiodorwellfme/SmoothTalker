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

        // Shot At
        public static ConfigEntry<bool> ShotAtEnabled { get; private set; }
        public static ConfigEntry<EPhraseTrigger> ShotAtTrigger { get; private set; }
        public static ConfigEntry<float> ShotAtRadius { get; private set; }
        public static ConfigEntry<float> ShotAtCooldown { get; private set; }
        public static ConfigEntry<bool> ShotAtSuppressInCombat { get; private set; }

        public static void Init(ConfigFile config)
        {
            const string general = "1. General";
            const string kills = "2. Kills";
            const string reload = "3. Reload";
            const string outOfAmmo = "4. Out of Ammo";
            const string grenade = "5. Grenade";
            const string shotAt = "6. Shot At";

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

            ShotAtEnabled = config.Bind(
                shotAt,
                "Enabled",
                true,
                new ConfigDescription(
                    "Play a voice line when an enemy bullet passes close to the player.",
                    null,
                    new global::ConfigurationManagerAttributes
                    {
                        Order = 3
                    }));

            ShotAtTrigger = config.Bind(
                shotAt,
                "Voice Trigger",
                EPhraseTrigger.OnBeingHurt,
                new ConfigDescription(
                    "Voice line to play when getting shot at.",
                    null,
                    new global::ConfigurationManagerAttributes
                    {
                        Order = 2
                    }));

            ShotAtRadius = config.Bind(
                shotAt,
                "Detection Radius (meters)",
                5f,
                new ConfigDescription(
                    "How close (in meters) a bullet must pass to trigger the voice line.\n" +
                    "Mirrors the perpendicular distance FlyingBulletSoundPlayer uses (~10m for near-miss audio).",
                    new AcceptableValueRange<float>(0.5f, 20f),
                    new global::ConfigurationManagerAttributes
                    {
                        Order = 3
                    }));

            ShotAtCooldown = config.Bind(
                shotAt,
                "Cooldown (seconds)",
                8f,
                new ConfigDescription(
                    "Minimum seconds between shot-at voicelines.\n" +
                    "This is separate from the global cooldown so it doesn't compete with kill or reload lines.",
                    new AcceptableValueRange<float>(1f, 60f),
                    new global::ConfigurationManagerAttributes
                    {
                        Order = 2
                    }));

            ShotAtSuppressInCombat = config.Bind(
                shotAt,
                "Suppress When In Combat",
                true,
                new ConfigDescription(
                    "Don't play the shot-at line while the player is actively shooting back.\n" +
                    "Uses the same Combat Timeout window as the other combat-only settings.",
                    null,
                    new global::ConfigurationManagerAttributes
                    {
                        Order = 1
                    }));
        }
    }
}