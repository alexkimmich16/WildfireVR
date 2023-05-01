using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BlockObject : MonoBehaviour
{
    public MeshCollider collider;

    public void EnableCollider(bool State)
    {
        collider.enabled = State;
    }
    private void Start()
    {
        collider.enabled = false;
    }

    private void OnEnable()
    {
        GetComponent<PhotonVFX>().SetVFX += EnableCollider;
    }
    private void OnDisable()
    {
        GetComponent<PhotonVFX>().SetVFX -= EnableCollider;
    }
}
