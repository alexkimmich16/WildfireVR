using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class BlockMimic : MonoBehaviour
{
    public GameObject Display;
    public GameObject Real;
    public VisualEffect effect;
    public bool UseDisplay;
    void Start()
    {
        Display.SetActive(UseDisplay);
    }

    // Update is called once per frame
    void Update()
    {
        CheckObject(Real.GetComponent<BoxCollider>());
    }
    void CheckObject(BoxCollider collider)
    {
        if(UseDisplay == true)
            Display.transform.position = transform.position - GetCenter();
        
        effect.SetVector3("Center", GetCenter());

        Vector3 RotVector = GetSize();
        effect.SetVector3("Size", RotVector);

        if (UseDisplay == true)
            Display.transform.rotation = GetRotation();
        //Test

        Vector3 GetCenter()
        {
            return collider.transform.position - transform.position;
        }
        Vector3 GetSize()
        {
            return collider.transform.localScale;
        }



        Quaternion GetRotation()
        {
            return collider.transform.rotation;
        }
    }
}
