using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum DisolveType
{
    MainPlayer = 0,
    NetworkPlayer = 1,
}
public class DisolveManager : MonoBehaviour
{
    public Material Mat;
    private Material NewMat;
    public bool ShouldDisolve = false;
    private bool DisolveSpare = true;
    private float CurrentTime;
    private float DivideNum;
    private float SinNum;
    public bool ShouldRegenerate;
    public DisolveType disolveType;
    public PlayerControl SubscribeDisolve;
    private bool Started = false;

    public void LookForSubscribe()
    {
        for (int i = 0; i < NetworkManager.instance.Players.Count; i++)
        {
            if (NetworkManager.instance.Players[i].ObjectReference.transform.GetComponent<NetworkPlayer>().photonView.IsMine)
            {
                SubscribeDisolve = NetworkManager.instance.Players[i].ObjectReference.transform.GetComponent<PlayerControl>();
                return;
            }
        }
    }
    public void TryInitialize()
    {
        if (SubscribeDisolve != null)
        {
            Started = true;
            SubscribeDisolve.disolveEvent += DisolveThis;
            DivideNum = 2 / PlayerControl.DeathTime;
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
    }
    void Start()
    {
        TryInitialize();
    }
    void Update()
    {
        if(Started == false)
        {
            LookForSubscribe();
            TryInitialize();
        }
        if (ShouldDisolve == true && Started == true)
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
                if(ShouldRegenerate == true)
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
