using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Sirenix.OdinInspector;
public class FadeManager : SerializedMonoBehaviour
{
    public static FadeManager instance;
    private void Awake() { instance = this; }

    public bool FadeRising;
    [ReadOnly]public float CurrentFadeValue;
    public float FadeSpeed;
    public Vector2 MaxMinFade;

    public Volume Volume;
    private void Start()
    {
        CurrentFadeValue = MaxMinFade.x;
        NetworkManager.DoFade += ChangeFade;
        SpawnManager.OnElevatorRespawn += OnStart;
    }
    public void OnStart() { FadeRising = true; }
    public void ChangeFade(bool In) { FadeRising = In; }
    void Update()
    {
        CurrentFadeValue += Time.deltaTime * FadeSpeed * (FadeRising ? 1 : -1);
        CurrentFadeValue = Mathf.Clamp(CurrentFadeValue, MaxMinFade.x, MaxMinFade.y);
        if (Volume.profile.TryGet<ColorAdjustments>(out ColorAdjustments Color))
        {
            Color.postExposure.overrideState = true;
            Color.postExposure.value = CurrentFadeValue;

        }
    }
}
