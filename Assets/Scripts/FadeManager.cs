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
    private Vector2 MaxMinFade;

    public bool DoFade;

    public Volume Volume;
    private void Start()
    { 
        SpawnManager.OnElevatorRespawn += OnStart;
        MaxMinFade.x = -10f;
        if (Volume.profile.TryGet<ColorAdjustments>(out ColorAdjustments Color))
            MaxMinFade = new Vector2(MaxMinFade.x, Color.postExposure.value);

        if (!DoFade)
            SetFade(Mathf.Clamp(100f, MaxMinFade.x, MaxMinFade.y));
        else
        {
            CurrentFadeValue = MaxMinFade.x;
            NetworkManager.DoFade += ChangeFade;
        }
    }
    public void OnStart() { FadeRising = true; }
    public void ChangeFade(bool In) { FadeRising = In; }
    void Update()
    {
        if (DoFade)
        {
            CurrentFadeValue += Time.deltaTime * FadeSpeed * (FadeRising ? 1 : -1);
            CurrentFadeValue = Mathf.Clamp(CurrentFadeValue, MaxMinFade.x, MaxMinFade.y);
            SetFade(CurrentFadeValue);
        }
        
    }

    public void SetFade(float NewValue)
    {
        if (Volume.profile.TryGet<ColorAdjustments>(out ColorAdjustments Color))
        {
            Color.postExposure.overrideState = true;
            Color.postExposure.value = NewValue;
        }
    }
}
