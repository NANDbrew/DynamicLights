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
        public const string PLUGIN_VERSION = "0.2.0";

        //--settings--
        internal static ConfigEntry<bool> globalShadows;
        internal static ConfigEntry<bool> interiorShadows;

        internal static ConfigEntry<bool> lightHalo;
        internal static ConfigEntry<int> vertexLightDistance;
        internal static ConfigEntry<int> shadowLightDistance;

        private void Awake()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), PLUGIN_ID);

            globalShadows = Config.Bind("Settings", "Exterior Shadows", false, new ConfigDescription("Dynamic shadows for outdoor lights"));
            interiorShadows = Config.Bind("Settings", "Interior shadows", false, new ConfigDescription("Dynamic shadows for indoor lights"));

            lightHalo = Config.Bind("Settings", "FFL Light Haloes", false, new ConfigDescription("Leave this off until Raw Lion fixes Fire Fish Town's performance issue"));
            vertexLightDistance = Config.Bind("Settings", "Vertex Light Distance", 45, new ConfigDescription("Reduce this for better performance, increase it to reduce pop-in", new AcceptableValueRange<int>(20, 300)));
            shadowLightDistance = Config.Bind("Settings", "Shadow Light Distance", 25, new ConfigDescription("Lights beyond this will not cast shadows", new AcceptableValueRange<int>(10, 100)));

        }
    }
}
