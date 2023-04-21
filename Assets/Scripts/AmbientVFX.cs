using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.SceneManagement;
public class AmbientVFX : MonoBehaviour
{
    void Awake() { instance = this; }
    public static AmbientVFX instance;

    public List<Transform> Actives = new List<Transform>();
    public float Inverval;
    public VisualEffect vfxGraph; // reference to the Unity VFX graph

    private float timer = 0.0f;

    public int VFXStorage;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= Inverval)
        {
            timer = 0.0f;
            UpdateVFXGraph();
        }
    }

    void UpdateVFXGraph()
    {
        for (int i = 0; i < VFXStorage; i++)
        {
            vfxGraph.SetVector3("PushPos" + i, i < Actives.Count ? Actives[i].position : FarAway());
            vfxGraph.SetVector3("PushDir" + i, i < Actives.Count ? Actives[i].forward : Vector3.zero);
        }
        
    }

    public Vector3 FarAway() { return new Vector3(1000f, 1000f, 1000f); }
}
