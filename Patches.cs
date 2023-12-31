﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;

namespace Dynamic_Lights
{
    internal class Patches
    {
        [HarmonyPatch(typeof(ShipItemLight))]
        private static class ShipItemLightPatches
        {
            [HarmonyPatch("OnLoad")]
            public static void Prefix(ref Material ___paperOffMat, Renderer ___paperRenderer)
            {
                if (___paperOffMat)
                {
                    ResourceRefs.paperOffMat = ___paperOffMat;

                    if (___paperRenderer.sharedMaterial.name.Contains("blu"))
                    {
                        Material bluePaper = ResourceRefs.paperOffMatBlue;
                        if (bluePaper == null || bluePaper.name == "lamp blue off") { bluePaper.CopyPropertiesFromMaterial(___paperOffMat); bluePaper.color = new Color(0.3676575f, 0.760615f, 0.9622641f, 1); }
                        ___paperOffMat = ResourceRefs.paperOffMatBlue;
                    }

                }

            }
        }

        [HarmonyPatch(typeof(IslandStreetlightsManager))]
        private static class IslandStreetLightsManagerpatches
        {
            [HarmonyPatch("LightDistanceCheckLoop")]
            [HarmonyPrefix]
            public static bool DistanceCheckLoopPatch(IslandStreetlightsManager __instance, ref List<IslandStreetlight> ___streetlights, ref int ___i)
            {
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

                    ___i++;
                }
                return false; 
            }


        }

        [HarmonyPatch(typeof(IslandSceneryScene))]
        private static class SomeClassPatch
        {
            [HarmonyPatch("Update")]
            [HarmonyPostfix]
            public static void Patch(IslandSceneryScene __instance, int ___parentIslandIndex)
            {
                if (!__instance.gameObject.GetComponent<IslandStreetlightsManager>() && ___parentIslandIndex > 8 && ___parentIslandIndex != 20)
                {
                    __instance.gameObject.AddComponent<IslandStreetlightsManager>();
                    if (___parentIslandIndex == 15)
                    {
                        __instance.GetComponent<IslandStreetlightsManager>().vertexLightDistance = 90;
                    }
                }
                if (!__instance.gameObject.GetComponent<LightManager>())
                {
                    __instance.gameObject.AddComponent<LightManager>();
                }

            }
        }
    }
}
