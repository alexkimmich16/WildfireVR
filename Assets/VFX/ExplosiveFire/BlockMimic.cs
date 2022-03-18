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
    public List<Transform> Shields;

    public static float CheckInterval = 0.1f;
    public static float CheckDistance = 5f;
    //trigger instead
    void Start()
    {
        Display.SetActive(UseDisplay);
        StartCoroutine(Wait());
    }
    public IEnumerator Wait()
    {
        yield return new WaitForSeconds(CheckInterval);
        CheckForShields();
        StartCoroutine(Wait());
    }

    private void Update()
    {
        int Max = 2;
        for (int i = 0; i < Shields.Count; i++)
        {
            string CenterString = "Center" + i;
            effect.SetVector3(CenterString, Shields[i].position);
        }
        int Left = Max - Shields.Count;
        for (int i = 0; i < Left; i++)
        {
            int Current = i + Shields.Count;
            string CenterString = "Center" + Current;
            effect.SetVector3(CenterString, AbsurdValue());
        }
    }
    public void CheckForShields()
    {
        Collider[] Colliders = Physics.OverlapSphere(transform.position, CheckDistance);
        List<Collider> TrueColliders = new List<Collider>();
        Shields.Clear();
        for (int i = 0; i < Colliders.Length; i++)
        {
            //Debug.Log("pt1");
            if (Colliders[i].transform.tag == "Shield" && Colliders[i].transform.GetComponent<ShieldManager>())
            {
                //Debug.Log("pt2");
                if (transform != Colliders[i].transform.parent)
                {
                    //Debug.Log("pt3");
                    if (Colliders[i].transform.GetComponent<ShieldManager>().ShieldActive == true)
                    {
                        //Debug.Log("pt4");
                        TrueColliders.Add(Colliders[i]);
                        Shields.Add(Colliders[i].transform);
                    }
                }
            }
                
                
                    
                    
        }
            
    }
    Vector3 AbsurdValue()
    {
        return new Vector3(1000, 1000, 1000);
    }
    Vector3 GetCenter(Vector3 pos)
    {
        return pos - transform.position;
    }
}
