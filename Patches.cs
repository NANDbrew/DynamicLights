using HarmonyLib;
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

        [HarmonyPatch(typeof(IslandStreetlight))]
        private static class IslandStreetlightPatches
        {
            [HarmonyPatch("Start")]
            [HarmonyPostfix]
            public static void StartPatch(IslandStreetlight __instance, ref Light ___light)
            {
                if (__instance.name == "senna_lantern_light")
                {
                    Debug.Log("hey man! i'm bullshit!");
                    GameObject lightContainer = UnityEngine.Object.Instantiate(__instance.gameObject, __instance.transform);
                    lightContainer.transform.localPosition = new Vector3(-0.012f, 0.0036f, 0.1436f);
                    lightContainer.transform.localRotation = new Quaternion(-0.6227f, -0.7812f, -0.0164f, -0.0406f);
                    UnityEngine.Object.Destroy(lightContainer.GetComponent<Rigidbody>());
                    UnityEngine.Object.Destroy(lightContainer.GetComponent<IslandStreetlight>());
                    UnityEngine.Object.Destroy(lightContainer.GetComponent<MeshRenderer>());
                    UnityEngine.Object.Destroy(lightContainer.GetComponent<MeshFilter>());
                    Light newLight = lightContainer.GetComponent<Light>();

                    newLight.shadowNearPlane = 0.2f;
                    //newLight.type = LightType.Spot;
                    newLight.spotAngle = 180;

                    UnityEngine.Object.Destroy(__instance.gameObject.GetComponent<Light>());
                    ___light = newLight;

                }

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
                }
                if (!__instance.gameObject.GetComponent<LightManager>())
                {
                    __instance.gameObject.AddComponent<LightManager>();
                }
            }
        }


        [HarmonyPatch(typeof(PortDude), "Awake")]
        private static class PortDudePatch
        {
            [HarmonyPostfix]
            public static void AddInteriorEffectsTrigger(PortDude __instance, Port ___port)
            {
                int portIndex = ___port.portIndex;
                // exclude outdoor offices
                if (portIndex != 5 && portIndex != 24)
                {
/*                    GameObject interiorTrigger = UnityEngine.Object.Instantiate(new GameObject() { name = "port interior trigger" }, __instance.transform.position, ResourceRefs.triggerRotations[portIndex], __instance.transform);
                    interiorTrigger.AddComponent<InteriorEffectsTrigger>();
                    interiorTrigger.transform.position = ResourceRefs.triggerLocs[portIndex];
                    BoxCollider bcol = interiorTrigger.AddComponent<BoxCollider>();
                    bcol.size = ResourceRefs.colSizes[portIndex];
                    bcol.isTrigger = true;*/

                    AddInteriorTrigger(__instance.transform, portIndex);

                    if (portIndex == 0) AddInteriorTrigger(__instance.transform, 30);
                    if (portIndex == 15) AddInteriorTrigger(__instance.transform, 31);
                    if (portIndex == 21) AddInteriorTrigger(__instance.transform, 32);
                    if (portIndex == 25) AddInteriorTrigger(__instance.transform, 33);
                    if (portIndex == 13) AddInteriorTrigger(__instance.transform, 34);
                }
            }
        }

        public static void AddInteriorTrigger(Transform parent, int index)
        {
            GameObject interiorTrigger = UnityEngine.Object.Instantiate(new GameObject() { name = "port interior trigger " + index }, parent.position, ResourceRefs.triggerRotations[index], parent);
            interiorTrigger.AddComponent<InteriorEffectsTrigger>();
            interiorTrigger.transform.localPosition = ResourceRefs.triggerLocs[index];
            BoxCollider bcol = interiorTrigger.AddComponent<BoxCollider>();
            bcol.size = ResourceRefs.colSizes[index];
            bcol.isTrigger = true;
        }
    }
}
