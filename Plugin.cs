using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Reflection;

namespace Dynamic_Lights
{
    [BepInPlugin(PLUGIN_ID, PLUGIN_NAME, PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public const string PLUGIN_ID = "com.nandbrew.dynamiclights";
        public const string PLUGIN_NAME = "Dynamic Lights";
        public const string PLUGIN_VERSION = "0.0.1";

        //--settings--
        internal static ConfigEntry<bool> globalShadows;
        internal static ConfigEntry<bool> sennaSpots;

        private void Awake()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), PLUGIN_ID);

            globalShadows = Config.Bind("Settings", "Lamp Shadows", true);
            //sennaSpots = Config.Bind("Settings", "Senna spotlights", true);
        }
    }
}
