using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisolveManager : MonoBehaviour
{
    public Material Mat;
    private Material NewMat;
    public bool ShouldDisolve = false;
    private bool DisolveSpare = true;
    private float CurrentTime;
    private float DivideNum;
    private float SinNum;
    public bool IsPlayer;
    //public event  disolveEvent;
    void Start()
    {
        
        DivideNum = 2 / PlayerControl.DeathTime;
        PlayerControl.disolveEvent += DisolveThis;
        NewMat = new Material(Mat);
        NewMat.SetFloat("Time", -1);
        if (gameObject.GetComponent<SkinnedMeshRenderer>())
        {
            gameObject.GetComponent<SkinnedMeshRenderer>().sharedMaterial = NewMat;
        }
        else if (gameObject.GetComponent<MeshRenderer>())
        {
            gameObject.GetComponent<MeshRenderer>().sharedMaterial = NewMat;
        }
    }
    void Update()
    {
        if (ShouldDisolve == true)
        {
            if(DisolveSpare == true)
            {
                CurrentTime = -1;
                DisolveSpare = false;
            }
            CurrentTime += Time.deltaTime * DivideNum;
            SinNum = Mathf.Sin(CurrentTime);
            //float ToSet = 
            NewMat.SetFloat("Time", SinNum);
            if (CurrentTime > 1)
            {
                ShouldDisolve = false;
                DisolveSpare = true;
                if(IsPlayer == true)
                    NewMat.SetFloat("Time", -1);
            }
        }
    }
    public void DisolveThis()
    {
        ShouldDisolve = true;
    }

    //disolve manager should send info to others
}
