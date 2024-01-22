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

        public float vertexLightDistance = 44f;

        private List<IslandStreetlightFire> streetlights = new List<IslandStreetlightFire>();
        private int i;
        private bool lightOn;
        private bool dayLightOn;

        private void Start()
        {
            ResourceRefs.CreateMaterials();
            Transform portDude = Refs.islands[gameObject.GetComponent<IslandSceneryScene>().parentIslandIndex].GetComponentInChildren<PortDude>().transform;

            foreach (Light light in gameObject.GetComponentsInChildren<Light>())
            {
                if (light.transform.parent.GetComponent<ShipItemLight>() || light.transform.parent.GetComponent<ShipItemStove>()) continue; // skip if we're a carryable lantern or a stove
                if (light.name.Contains("halo")) continue;

                if (light.transform.parent.name.Contains("stove"))
                {
                    light.shadowStrength = 1.0f;
                }
                else
                {
                light.shadowStrength = 0.75f;
                }

                if (light.name.Contains("senna")) // these are also present at on'na
                {
                    light.shadowNearPlane = 0f;
                    if (GetComponent<IslandSceneryScene>().parentIslandIndex == 28) // sen'na 
                    { 
                        light.shadowResolution = LightShadowResolution.Low;
                    }

                }
                else if (light.name.Contains("street") || light.name.Contains("Particle") || light.name.Contains("Cube")) // 'street' is ffl, emerald, aestrin. 'Particle' and 'Cube' are al'ankh
                {
                    if (light.name.Contains("east"))
                    {
                        light.shadowNearPlane = 0.37f;
                    }
                    else
                    {
                        light.shadowNearPlane = 0.6f;
                    }
                }
                else if (light.transform.parent.name.Contains("candle"))
                {
                    light.shadowNearPlane = 0.05f;
                }

                if (light.name.Contains("Particle") && GetComponent<IslandSceneryScene>().parentIslandIndex == 27) // kicia altar
                {
                    light.shadows = LightShadows.Soft;
                    continue;
                }

                IslandStreetlightFire streetlight = light.gameObject.AddComponent<IslandStreetlightFire>(); // new class handles light with sound/particles
                streetlight.lightManager = this;
                streetlight.halo = light.transform.parent.GetComponentInChildren<LightHalo>();

                if (Vector3.Distance(light.transform.position, portDude.position) < 5)
                {
                    streetlight.type = LightType.Always;
                }
                else if (light.transform.parent.name.Contains("stove"))
                {
                    if (light.transform.parent.name.Contains("shop stove E")) 
                    {
                        var pos = light.transform.localPosition;
                        pos.y = 0.097f;
                        light.transform.localPosition = pos;
                    }
                    streetlight.type = LightType.Day;
                    streetlight.audioSource = streetlight.transform.parent.GetComponentInChildren<AudioSource>();
                }
                else if (light.transform.parent.name.Contains("candle"))
                {
                    streetlight.type = LightType.Day;
                }
                else
                {
                    streetlight.type = LightType.Night;
                }

                if (light.gameObject.GetComponent<IslandStreetlight>() is IslandStreetlight component)
                {
                    streetlight.offMat = component.offMat;
                    streetlight.halo = component.halo;
                    streetlight.ffl = true;
                    if (GetComponent<IslandSceneryScene>().parentIslandIndex == 28)
                    {
                        streetlight.senna = true;
                    }
                    UnityEngine.Object.Destroy(component);
                }

                if (streetlight.offMat == null)
                {
                    if (streetlight.GetComponent<Renderer>().materials[0].name.Contains("green"))
                    {
                        streetlight.offMat = ResourceRefs.paperOffMatGreen;
                    }
                    else if (streetlight.GetComponent<Renderer>().materials[0].name.Contains("red"))
                    {
                        streetlight.offMat = ResourceRefs.paperOffMatRed;
                    }
                    else if (streetlight.GetComponent<Renderer>().materials[0].name.Contains("blu"))
                    {
                        streetlight.offMat = ResourceRefs.paperOffMatBlue;
                    }
                    else if (streetlight.GetComponent<Renderer>().materials[0].name.Contains("yellow"))
                    {
                        streetlight.offMat = ResourceRefs.paperOffMatYellow;
                    }
                }
                
            }
        }

        private void Update()
        {
            vertexLightDistance = (float)Plugin.vertexLightDistance.Value;
 
            if (Sun.sun.localTime > 16f || Sun.sun.localTime < 8f)
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
                if (Vector3.Distance(streetlights[i].transform.position, Camera.main.transform.position) > vertexLightDistance)
                {
                    streetlights[i].GetLight().renderMode = LightRenderMode.ForceVertex;
                }
                else
                {
                    streetlights[i].GetLight().renderMode = LightRenderMode.ForcePixel;
                }

                if ((Plugin.globalShadows.Value || (Plugin.sennaShadows.Value && streetlights[i].senna)) && Vector3.Distance(streetlights[i].transform.position, Camera.main.transform.position) < Plugin.shadowLightDistance.Value)
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
