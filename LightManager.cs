using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace Dynamic_Lights
{
    internal class LightManager : MonoBehaviour
    {
        // Yes, I'm aware this code is a mess. Don't judge me, it works.

        //public bool sennaShadows;
        //public bool shadowsOn;
        //private List<GameObject> dayLights = new List<GameObject>();
        //private Light[] lights;
        //private List<Light> shadowLights = new List<Light>();
        //private List<Light> sennaLights = new List<Light>();

        //public float vertexLightDistance = 44f;

        private List<IslandStreetlightFire> streetlights = new List<IslandStreetlightFire>();
        private int i;
        private bool lightOn;
        private bool dayLightOn;

        private int islandIndex;

        private void Start()
        {
            //ResourceRefs.CreateMaterials();
            islandIndex = GetComponent<IslandSceneryScene>().parentIslandIndex;


            foreach (Light light in gameObject.GetComponentsInChildren<Light>())
            {
                if (light.transform.parent.GetComponent<ShipItemLight>() || light.transform.parent.GetComponent<ShipItemStove>()) continue; // skip if we're a carryable lantern or a stove
                if (light.name.Contains("halo")) continue;
                if (islandIndex == 30) // rock of despair
                {
                    light.shadows = LightShadows.Soft;
                    light.shadowStrength = 0.75f;

                    continue;
                }
                if (light.name.Contains("Particle") && !light.transform.parent.name.Contains("stove") && islandIndex == 27) // kicia altar
                {
                    light.shadows = LightShadows.Soft;
                    light.shadowStrength = 0.75f;

                    continue;
                }

                IslandStreetlightFire streetlight = light.gameObject.AddComponent<IslandStreetlightFire>(); // new class handles daytime lights and special cases
                streetlight.lightManager = this;
                if (light.gameObject.GetComponent<IslandStreetlight>() is IslandStreetlight component)
                {
                    streetlight.offMat = component.offMat;
                    streetlight.halo = component.halo;
                    streetlight.particleSystem = component.particles;
                    streetlight.audioSource = component.audio;
                    UnityEngine.Object.Destroy(component);

                    if (light.name.Contains("east"))
                    {
                        light.shadowNearPlane = 0.37f;
                    }
                    else
                    {
                        light.shadowNearPlane = 0.6f;
                    }
                    streetlight.type = LightType.Night;
                }
                else
                {
                    streetlight.type = LightType.Always;
                }

                if (light.transform.parent.name.Contains("stove"))
                {
                    if (light.transform.parent.name.Contains("shop stove E"))
                    {
                        var pos = light.transform.localPosition;
                        pos.y = 0.097f;
                        light.transform.localPosition = pos;
                    }
                    streetlight.type = LightType.Day;
                    streetlight.audioSource = streetlight.transform.parent.GetComponentInChildren<AudioSource>();
                    light.shadowStrength = 1.0f;

                }
                else if (light.transform.parent.name.Contains("candle"))
                {
                    light.shadowNearPlane = 0.05f;
                    streetlight.type = LightType.Day;
                    streetlight.interior = true;
                    light.shadowStrength = 0.75f;

                }
                else
                {
                    light.shadowStrength = 0.75f;
                }

                if (islandIndex >= 26 && islandIndex <= 29) // FFL
                {
                    if (light.name.Contains("senna")) // these are also present at on'na
                    { 
                        light.shadowNearPlane = 0f;
                    }
                    light.shadowResolution = LightShadowResolution.Low;
                    streetlight.ffl = true;
                }

              
            }
        }

        private void Update()
        {
            if (GameState.justStarted)
            {
                foreach (IslandStreetlightFire streetlight in streetlights)
                {
                    if (Vector3.Distance(streetlight.transform.position, Refs.islands[islandIndex].GetComponentInChildren<PortDude>().transform.position) < 5)
                    {
                        streetlight.type = LightType.Always;
                    }
                }
            }

            if (Sun.sun.localTime > 16.5f || Sun.sun.localTime < 7.5f)
            {
                lightOn = true;
            }
            else
            {
                lightOn = false;
            }


            if (Sun.sun.localTime > 18f || Sun.sun.localTime < 7f)
            {
                dayLightOn = false;
            }
            else
            {
                dayLightOn = true;
            }
            LightDistanceCheckLoop();
      
        }

        public void AddStreetlight(IslandStreetlightFire light)
        {
            if (streetlights == null)
            {
                streetlights = new List<IslandStreetlightFire>();
            }

            streetlights.Add(light);
        }
        private void LightDistanceCheckLoop()
        {
            if (streetlights.Count > 0)
            {
                if (i >= streetlights.Count)
                {
                    i = 0;
                }
                if (Vector3.Distance(streetlights[i].transform.position, Camera.main.transform.position) > Plugin.vertexLightDistance.Value)
                {
                    streetlights[i].GetLight().renderMode = LightRenderMode.ForceVertex;
                }
                else
                {
                    streetlights[i].GetLight().renderMode = LightRenderMode.ForcePixel;
                }

                if ((Plugin.globalShadows.Value || (Plugin.interiorShadows.Value && streetlights[i].interior)) && Vector3.Distance(streetlights[i].transform.position, Camera.main.transform.position) < Plugin.shadowLightDistance.Value)
                {
                    streetlights[i].GetLight().shadows = LightShadows.Soft;
                }
                else streetlights[i].GetLight().shadows = LightShadows.None;


                if (streetlights[i].type == LightType.Day)
                {
                    streetlights[i].SetLight(dayLightOn);
                }
                else if (streetlights[i].type == LightType.Night)
                {
                    streetlights[i].SetLight(lightOn);
                }

                i++;
            }
        
        }
    }
}
