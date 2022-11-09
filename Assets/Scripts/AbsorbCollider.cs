using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbsorbCollider : MonoBehaviour
{
    public Side side;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.name);
        if (other.GetComponent<Fireball>())
        {
            AIMagicControl.instance.Absorbs[(int)side].OnAbsorbStart(other.transform);
        }
    }
}
