using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmbientParticles;
public enum Quality
{
    Low = 0,
    Medium = 1,
    High = 2,
}
public class SettingsControl : MonoBehaviour
{
    public static Quality quality { 
        get { return (Quality)QualitySettings.GetQualityLevel(); } 
        set {
            QualitySettings.SetQualityLevel((int)value);
        } }

    //settings
    public Quality FogMinQuality;
    public float[] AmbientParticles;

    //references
    public GameObject FogObject;

    public delegate void SettingsChange(Quality quality);
    public static event SettingsChange OnSettingsChange;
    void Start()
    {
        OnSettingsChange += NewSettings;
        quality = (Quality) PlayerPrefs.GetInt("quality");
    }
    public void NewSettings(Quality quality)
    {
        //Debug.Log(quality.ToString());
        FogObject.SetActive(quality >= FogMinQuality);
        AmbientVFX.instance.vfxGraph.SetFloat("SpawnAmount", AmbientParticles[(int)quality]);

    }

    public static void ChangeSettings(int Levels)
    {
        quality = (Quality)Mathf.Clamp(Levels + (int)quality, 0, 2);
        PlayerPrefs.SetInt("quality", (int)quality);
        OnSettingsChange?.Invoke(quality);
    }
}
