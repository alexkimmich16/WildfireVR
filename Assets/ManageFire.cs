using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;
public class ManageFire : MonoBehaviour
{
    public float Speed;
    public VisualEffect Fire;

    private void Update()
    {
        Fire.playRate = Speed;
    }
}
