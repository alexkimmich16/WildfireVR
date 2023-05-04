using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
//using UnityEngine.Rendering.PostProcessing;
//using UnityEngine.Rendering.Universal;
namespace Effects
{
    public class DamageFlash : MonoBehaviour
    {
        public static DamageFlash instance;
        void Awake() { instance = this; }
        [Range(0, 1)]
        public float MaxAlpha;

        [Range(0, 1)]
        public float Falloff;

        public Color color;

        [Range(0, 1)]
        public float CurrentAlpha;

        public Volume Volume;

        private void Start()
        {
            NetworkManager.OnTakeDamage += DisplayFlash;
        }
        public void DisplayFlash()
        {
            CurrentAlpha = MaxAlpha;
        }
        void Update()
        {
            CurrentAlpha -= Falloff * Time.deltaTime;
            //image.color = new Color(color.r, color.g, color.b, CurrentAlpha);
            if (Volume.profile.TryGet<ChromaticAberration>(out var Chrom))
            {
                Chrom.intensity.overrideState = true;
                Chrom.intensity.value = CurrentAlpha;
            }
        }
    }
}

