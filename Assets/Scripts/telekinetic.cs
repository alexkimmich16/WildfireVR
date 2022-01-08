using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class telekinetic : MonoBehaviour
{
    public bool Touched;
    public Material Active;
    public Material Spare;

    private void Start()
    {
        Spare = transform.GetComponent<Renderer>().material;
    }

}
