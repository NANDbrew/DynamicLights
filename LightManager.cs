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
        // Yes, I'm aware this code is a mess. Don't judge me. it works.

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
            Transform portDude = Refs.islands[gameObject.GetComponent<IslandSceneryScene>().parentIslandIndex].GetComponentInChildren<PortDude>().transform;


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
                    
                    //component.enabled = false;
                    //UnityEngine.Object.Destroy(component);

                    light.shadowNearPlane = 0.6f;

                    streetlight.type = LightType.Night;

                    if (streetlight.transform.parent.name == "110 lantern A")
                    {
                        if (ResourceRefs.flame == null) ResourceRefs.CreateMaterials();
                        ParticleSystemRenderer renderer = streetlight.GetComponent<ParticleSystemRenderer>();

                        renderer.material.mainTexture = ResourceRefs.flame;
                        renderer.renderMode = ParticleSystemRenderMode.VerticalBillboard;
                        renderer.sortMode = ParticleSystemSortMode.YoungestInFront;
                        renderer.material.mainTextureOffset = new Vector2(0f, -0.6f);
                        renderer.material.mainTextureScale = new Vector2(1f, 1.5f);

                        var mn = streetlight.particleSystem.main;
                        mn.startRotation = 0f;
                        mn.startSize = 0.3f;
                        mn.startSpeed = 0.02f;
                        mn.startLifetime = 1f;
                        mn.startColor = new Color(1f, 0.646f, 0.285f);

                        /*var newObj = UnityEngine.Object.Instantiate(streetlight.gameObject, streetlight.transform.parent);
                        newObj.GetComponent<ParticleSystemRenderer>().enabled = false;
                        newObj.GetComponent<Light>().enabled = false;
                        newObj.GetComponent<ParticleSystem>().Stop();
                        newObj.GetComponent<IslandStreetlight>().enabled = false;
                        newObj.GetComponent<IslandStreetlightFire>().enabled = false;
                        streetlight.gameObject.GetComponent<MeshRenderer>().enabled = false;
                        streetlight.transform.localPosition = new Vector3(0f, -0.25f, 0f);*/



                        //var ns = streetlight.particleSystem.noise;
                        //ns.strengthMultiplier = 0.01f;
                        //var sh = streetlight.particleSystem.shape;
                        //sh.enabled = false;
                    }
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
                    light.shadowStrength = 1.0f;

                }
                else if (light.transform.parent.name.Contains("candle"))
                {
                    if (light.transform.parent.name.Contains("candle M"))
                    {
                        light.shadowNearPlane = 0.1f;

                        ParticleSystemRenderer renderer = streetlight.GetComponent<ParticleSystemRenderer>();

                        if (ResourceRefs.flame == null) ResourceRefs.CreateMaterials();
                        renderer.material.mainTexture = ResourceRefs.flame;
                        renderer.renderMode = ParticleSystemRenderMode.VerticalBillboard;
                        renderer.sortMode = ParticleSystemSortMode.YoungestInFront;

                        var mn = streetlight.particleSystem.main;
                        mn.startRotation = 0f;
                        mn.startSize = 0.3f;
                        mn.startSpeed = 0.02f;
                        mn.startColor = light.color;
                        light.transform.localPosition = new Vector3(0f, 0f, 1.39f);

                        var cl = streetlight.particleSystem.colorOverLifetime;
                        cl.enabled = false;
                        var ns = streetlight.particleSystem.noise;
                        //ns.strengthMultiplier = 0.01f;
                        var sh = streetlight.particleSystem.shape;
                        sh.enabled = false;
                    }
                    else light.shadowNearPlane = 0.25f;
                    
                    light.shadowStrength = 0.75f;
                    streetlight.type = LightType.Always;
                    streetlight.interior = true;

                }
                else
                {
                    light.shadowNearPlane = 0.4f;
                    light.shadowStrength = 0.75f;
                    streetlight.type = LightType.Always;
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

/*                if (Vector3.Distance(light.transform.position, portDude.position) < 5)
                {
                    streetlight.type = LightType.Always;
                    streetlight.interior = true;
                }*/
              
            }
        }

        private void Update()
        {
/*            if (GameState.justStarted)
            {
                foreach (IslandStreetlightFire streetlight in streetlights)
                {
                    if (Vector3.Distance(streetlight.transform.position, Refs.islands[islandIndex].GetComponentInChildren<PortDude>().transform.position) < 5)
                    {
                        streetlight.type = LightType.Always;
                    }
                }
            }*/

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
