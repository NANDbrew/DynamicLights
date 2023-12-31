﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace Dynamic_Lights
{
    internal class LightManager : MonoBehaviour
    {
        // Yes, I'm aware this code is a mess. Don't judge me, it works.

        public bool sennaShadows;
        public bool shadowsOn;
        private List<GameObject> dayLights = new List<GameObject>();
        private Light[] lights;
        private List<Light> shadowLights = new List<Light>();
        private List<Light> sennaLights = new List<Light>();

        public float vertexLightDistance = 44f;

        private List<IslandStreetlightFire> streetlights = new List<IslandStreetlightFire>();
        private int i;

        private void Start()
        {
            ResourceRefs.CreateMaterials();
            Transform portDude = Refs.islands[gameObject.GetComponent<IslandSceneryScene>().parentIslandIndex].GetComponentInChildren<PortDude>().transform;
            lights = gameObject.GetComponentsInChildren<Light>();

            foreach (Light light in lights)
            {
                if (light.transform.parent.GetComponent<ShipItemLight>()) continue; // skip if we're a carryable lantern
                string lightName = light.transform.parent.name;

                if (!light.name.Contains("halo") && light.transform.position.y < 20) // add to shadow list if we're not a halo or up on a hill
                {
                    light.shadowStrength = 0.75f;
                    if (light.name.Contains("senna"))
                    {
                        light.shadowNearPlane = 0f;
                        light.shadowResolution = LightShadowResolution.Low;
                        if (GetComponent<IslandSceneryScene>().parentIslandIndex == 28) // if we're in sen'na, put in different list and skip the component stage
                        {
                            sennaLights.Add(light);
                            continue; 
                        }

                    }
                    else if (light.name.Contains("street") || light.name.Contains("Particle") || light.name.Contains("Cube"))
                    {
                        light.shadowNearPlane = 0.6f;

                    }
                    else if (light.transform.parent.name.Contains("candle"))
                    {
                        light.shadowNearPlane = 0.05f;
                    }

                    shadowLights.Add(light);
                }


                if (Vector3.Distance(light.transform.position, portDude.position) < 5) continue; // skip if we're near port dude
                    
                if (lightName.Contains("stove") || lightName.Contains("candle")) dayLights.Add(light.gameObject); // stoves and candles are assumed to be shops.
                else if (!light.gameObject.GetComponent<IslandStreetlight>() && !light.gameObject.GetComponent<IslandStreetlightFire>()) // FFL stuff already has this component
                {
                    if (light.name.Contains("street")) // emerald/aestrin street lamps 
                    {
                        IslandStreetlight streetlight = light.gameObject.AddComponent<IslandStreetlight>();

                        gameObject.GetComponent<IslandStreetlightsManager>().AddStreetlight(streetlight);
                        streetlight.streetlightManager = gameObject.GetComponent<IslandStreetlightsManager>();
                        streetlight.halo = light.transform.parent.GetComponentInChildren<LightHalo>();

                        if (streetlight.GetComponent<Renderer>().materials[0].name.Contains("green")) 
                        {
                            streetlight.offMat = ResourceRefs.paperOffMatGreen; 
                        }
                        else if (streetlight.GetComponent<Renderer>().materials[0].name.Contains("red")) 
                        {
                            streetlight.offMat = ResourceRefs.paperOffMatRed; 
                        }
                        else if (streetlight.GetComponent<Renderer>().materials[0].name.Contains("yellow")) 
                        {
                            streetlight.offMat = ResourceRefs.paperOffMatYellow; 
                        }
                    }

                    else if (light.name.Contains("Cube") || light.name.Contains("Particle")) // al'ankh lanterns & braziers
                    {
                        IslandStreetlightFire streetlight = light.gameObject.AddComponent<IslandStreetlightFire>(); // new class handles light with sound/particles
                        this.AddStreetlight(streetlight);
                        streetlight.lightManager = this;
                        streetlight.halo = light.transform.parent.GetComponentInChildren<LightHalo>();

                    }
                }
                
            }
        }
        private void Update()
        {

            // set shadows, but only if something's changed
            if (GameState.justStarted || shadowsOn != Plugin.globalShadows.Value)
            {
                foreach (Light light in shadowLights)
                {
                    if (Plugin.globalShadows.Value)
                    {

                        light.shadows = LightShadows.Soft;
                    }
                    else
                    {
                        light.shadows = LightShadows.None;
                    }
                }
                shadowsOn = Plugin.globalShadows.Value;
            }

            // set sen'na specific shadows
            if (GameState.justStarted || sennaShadows != Plugin.sennaShadows.Value)
            {
                foreach (Light light in sennaLights)
                {
                    if (Plugin.sennaShadows.Value)
                    {

                        light.shadows = LightShadows.Soft;
                    }
                    else
                    {
                        light.shadows = LightShadows.None;
                    }
                }
                sennaShadows = Plugin.sennaShadows.Value;
            }

            LightDistanceCheckLoop();

            bool lightOn = false;
            if (Sun.sun.localTime > 16f || Sun.sun.localTime < 8f)
            {
                lightOn = true;
            }

            foreach (IslandStreetlightFire streetlight in streetlights)
            {
                streetlight.SetLight(lightOn);
            }

            bool dayLightOn = true;
            if (Sun.sun.localTime > 18f || Sun.sun.localTime < 7f)
            {
                dayLightOn = false;
            }
            foreach (GameObject light in dayLights) 
            {
                light.SetActive(dayLightOn);
                if (light.transform.parent.GetComponentInChildren<AudioSource>()) light.transform.parent.GetComponentInChildren<AudioSource>().mute = !dayLightOn;
            }
       
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

                i++;
            }
        }
    }
}
