using HarmonyLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;

namespace Dynamic_Lights
{
    internal class Patches
    {
/*        [HarmonyPatch(typeof(WanderingStormLightning))]
        private static class LightningPatches
        {
            [HarmonyPatch("LightningStrike")]
            public static void Prefix(Light ___light)
            {
                if (Plugin.globalShadows.Value)
                {
                    ___light.shadows = LightShadows.Hard;
                    ___light.shadowStrength = Plugin.lightningShadow.Value;
                }
                else
                {
                    ___light.shadows = LightShadows.None;
                }
            }
        }*/

        [HarmonyPatch(typeof(IslandStreetlightsManager))]
        private static class IslandStreetLightsManagerpatches
        {
            [HarmonyPatch("Awake")]
            [HarmonyPrefix]
            public static void UpdatePatch(IslandStreetlightsManager __instance)
            {
                __instance.gameObject.AddComponent<LightManager>();
                __instance.enabled = false;
                //UnityEngine.Object.Destroy(__instance);
            }

/*            [HarmonyPatch("Update")]
            [HarmonyPrefix]
            public static bool DistanceCheckLoopPatch(IslandStreetlightsManager __instance, ref List<IslandStreetlight> ___streetlights, ref int ___i)
            {
                __instance.vertexLightDistance = Plugin.vertexLightDistance.Value;
                bool light = false;
                if (Sun.sun.localTime > 16.5f || Sun.sun.localTime < 7.5f)
                {
                    light = true;
                }
                
                if (___streetlights.Count > 0)
                {
                    if (___i >= ___streetlights.Count)
                    {
                        ___i = 0;
                    }

                    if (Vector3.Distance(___streetlights[___i].transform.position, Camera.main.transform.position) > __instance.vertexLightDistance)
                    {
                        ___streetlights[___i].GetLight().renderMode = LightRenderMode.ForceVertex;
                    }
                    else
                    {
                        ___streetlights[___i].GetLight().renderMode = LightRenderMode.ForcePixel;
                    }
                    ___streetlights[___i].SetLight(light);

                    ___i++;
                }
                return false; 
            }*/


        }

        /*        [HarmonyPatch(typeof(LightHalo))]
                private static class HaloPatch
                {
                    [HarmonyPatch("ToggleHalo")]
                    [HarmonyPrefix]
                    public static bool Patch(ref bool ___haloActive)
                    {
                        if (Plugin.lightHalo.Value)
                        {
                            return true;
                        }
                        else
                        {
                            ___haloActive = false;
                            return false;
                        }
                    }
                }*/

/*        [HarmonyPatch(typeof(IslandSceneryScene))]
        private static class SomeClassPatch
        {
            [HarmonyPatch("Update")]
            [HarmonyPostfix]
            public static void Patch(IslandSceneryScene __instance)
            {
                if (__instance.gameObject.GetComponent<IslandStreetlightsManager>() is IslandStreetlightsManager component)
                {
                    component.enabled = !Plugin.globalToggle.Value;
                }
                if (__instance.gameObject.GetComponent<LightManager>() is LightManager component2)
                {
                    component2.enabled = Plugin.globalToggle.Value;
                }

            }
        }*/
    }
}
