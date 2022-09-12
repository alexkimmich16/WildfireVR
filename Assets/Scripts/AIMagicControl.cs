using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMagicControl : MonoBehaviour
{
    public List<Transform> PositionObjectives;
    public List<Transform> Hands;
    public List<Transform> Spawn;

    public static AIMagicControl instance;
    void Awake() { instance = this; }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
