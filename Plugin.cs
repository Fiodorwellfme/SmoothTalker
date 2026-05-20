using BepInEx;
using BepInEx.Logging;
using EFT;
using System;

namespace SmoothTalker
{
    [BepInPlugin("com.fiodor.smoothtalker", "SmoothTalker", "1.2.0")]
    public sealed class Plugin : BaseUnityPlugin
    {
        internal static Plugin Instance;
        internal static ManualLogSource LogSource;

        private void Awake()
        {
            Instance = this;
            LogSource = Logger;
            Settings.Init(Config);
            FikaDetection.Init();
            new Patches.ShotFiredPatch().Enable();
            new Patches.OnKillPatch().Enable();
            new Patches.ReloadPatch().Enable();
            new Patches.DryShotPatch().Enable();
            new Patches.GrenadeHighThrowPatch().Enable();
            new Patches.GrenadeLowThrowPatch().Enable();
            new Patches.ShotAtPatch().Enable();
            if (FikaDetection.FikaInstalled)
            {
                new Patches.FikaReloadPatch().Enable();
                new Patches.ReceivePingPatch().Enable();
                new Patches.ClientSendPingPatch().Enable();
                new Patches.ServerSendPingPatch().Enable();
                new Patches.ClientEnemySpottedPatch().Enable();
                new Patches.ServerEnemySpottedPatch().Enable();
            }
            
            Logger.LogInfo("SmoothTalker v1.2.0 loaded.");
        }


    }
}
