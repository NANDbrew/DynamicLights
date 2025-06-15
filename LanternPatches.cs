using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace Dynamic_Lights
{
    [HarmonyPatch(typeof(ShipItemLight))]
    internal static class LanternPatches
    {
        [HarmonyPatch("OnLoad")]
        [HarmonyPostfix]
        public static void LanternPatch(ShipItemLight __instance, ParticleSystem ___particles, Light ___light)
        {
            if (___particles == null) return;

            if (ResourceRefs.flame == null) ResourceRefs.CreateMaterials();

            var renderer = ___particles.GetComponent<ParticleSystemRenderer>();
            renderer.sharedMaterial.mainTexture = ResourceRefs.flame;
            renderer.renderMode = ParticleSystemRenderMode.VerticalBillboard;
            renderer.sortMode = ParticleSystemSortMode.YoungestInFront;

            var mn = ___particles.main;
            mn.startRotation = 0f;
            mn.startSize = 0.15f;
            mn.startSpeed = 0.02f;
            mn.startLifetime = 1f;
            mn.startColor = ___light.color;

            var ns = ___particles.noise;
            ns.enabled = false;
            //var sh = ___particles.shape;
            //sh.enabled = false;

            /*var newObj = UnityEngine.Object.Instantiate(___particles.gameObject, ___particles.transform.parent);
            newObj.GetComponent<ParticleSystemRenderer>().enabled = false;
            newObj.GetComponent<Light>().enabled = false;
            newObj.GetComponent<ParticleSystem>().Stop();
            ___particles.gameObject.GetComponent<MeshRenderer>().enabled = false;*/

            if (__instance.transform.name.Contains("lantern A"))
            {
                //___particles.transform.localPosition = new Vector3(0f, -0.24f, 0f);
                renderer.material.mainTextureOffset = new Vector2(0f, -0.5f);
                renderer.material.mainTextureScale = new Vector2(1f, 1.4f);
            }
            if (__instance.transform.name.Contains("lantern M"))
            {
                mn.startSize = 0.26f;

                //___particles.transform.localPosition = new Vector3(0f, -0.34f, 0f);
                //___particles.transform.localScale = new Vector3(1f, 0.7f, 1f);
                renderer.material.mainTextureOffset = new Vector2(0.0f, -0.3f);
                renderer.material.mainTextureScale = new Vector2(1f, 1.2f);

            }
        }
    }
}
