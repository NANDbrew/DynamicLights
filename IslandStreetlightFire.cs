using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Dynamic_Lights
{
    internal class IslandStreetlightFire : MonoBehaviour
    {
        public LightHalo halo;
        private Light light;
        private ParticleSystem particleSystem;
        private AudioSource audioSource;
        public LightManager lightManager;

        public Light GetLight()
        {
            return light;
        }

        private void Awake()
        {
            light = GetComponent<Light>();
            particleSystem = GetComponent<ParticleSystem>();
            audioSource = GetComponent<AudioSource>();
        }

        public void SetLight(bool newState)
        {
            light.enabled = newState;
            particleSystem.enableEmission = newState;
            if (audioSource) audioSource.mute = !newState;
            halo.ToggleHalo(newState);
        }

        private void Start()
        {
            if (lightManager)
            {
                lightManager.AddStreetlight(this);
            }
            if (!halo) halo = transform.parent.parent.GetComponentInChildren<LightHalo>();
        }
    }
}
