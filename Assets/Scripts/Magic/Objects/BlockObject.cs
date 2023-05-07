using UnityEngine;

public class BlockObject : SpellObjectClass
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

    protected override void OnEnable()
    {
        base.OnEnable();
        GetComponent<PhotonVFX>().SetVFX += EnableCollider;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        GetComponent<PhotonVFX>().SetVFX -= EnableCollider;
    }

    public override void SetAudio(bool State)
    {
        //One day
    }
}
