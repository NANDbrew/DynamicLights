using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Dynamic_Lights
{
    internal class IslandStreetlightFire : MonoBehaviour
    {
        public LightType type;
        public LightHalo halo;
        private Light light;
        public bool interior;

        public Material offMat;
        private Renderer renderer;
        private Material onMat;
        public bool ffl;

        public ParticleSystem particleSystem;
        public AudioSource audioSource;
        public LightManager lightManager;

        public Light GetLight()
        {
            return light;
        }

        private void Awake()
        {
            light = GetComponent<Light>();
            if (!particleSystem) particleSystem = GetComponent<ParticleSystem>();
            audioSource = GetComponent<AudioSource>();
            renderer = GetComponent<Renderer>();
            onMat = renderer.sharedMaterials[0];
        }

        public void SetLight(bool newState)
        {
            light.enabled = newState;

            if (particleSystem is ParticleSystem component)
            {
                if (newState == true)
                {
                    component.Play();
                }
                else
                {
                    component.Stop();
                }
            }
            else
            {
                Material[] sharedMaterials = renderer.sharedMaterials;
                if (newState) sharedMaterials[0] = onMat;
                else sharedMaterials[0] = offMat;
                renderer.sharedMaterials = sharedMaterials;

            }

            if (audioSource) audioSource.mute = !newState;
            if (halo)
            {
                if (ffl && !Plugin.lightHalo.Value) halo.ToggleHalo(false);
                else halo.ToggleHalo(newState);
            }
        }

        private void Start()
        {
            if (lightManager)
            {
                lightManager.AddStreetlight(this);
            }
            //if (!halo) halo = transform.parent.parent.GetComponentInChildren<LightHalo>();
        }
    }
}
