using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageFlash : MonoBehaviour
{
    public static DamageFlash instance;
    void Awake() { instance = this; }
    [Range(0,1)]
    public float MaxAlpha;

    [Range(0, 1)]
    public float Falloff;

    public Color color;

    [Range(0, 1)]
    public float CurrentAlpha;
    public Image image;

    public bool Flash;
    public void DisplayFlash()
    {
        CurrentAlpha = MaxAlpha;
    }
    void Update()
    {
        CurrentAlpha -= Falloff * Time.deltaTime;
        image.color = new Color(color.r, color.g, color.b, CurrentAlpha);

        if(Flash == true)
        {
            Flash = false;
            DisplayFlash();
        }
    }
}
