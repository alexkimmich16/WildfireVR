using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockPosition : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform Other;
    public bool Active;
    // Update is called once per frame
    void Update()
    {
        if (Other != null && Active)
            transform.position = new Vector3(Other.position.x, transform.position.y, Other.position.z);
    }
}
