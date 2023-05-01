using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Odin.Net;
using Photon.Pun;

public class DecalSpawner : MonoBehaviour
{
    public static DecalSpawner instance;
    private void Awake() { instance = this; }
    public float SphereCastSize = 0.4f;
    public float FloorHeight;
    public bool SpawnDecals;

    public float DistanceToWall;
    public float Moveback;

    public bool After;
    public bool Before;

    
    private void SpawnDecalAtPosition(Vector3 Pos, Quaternion Rot)
    {
        if (SpawnDecals)
        {
            //Debug.Log("Pos: " + Pos + "  Count: " + Physics.OverlapSphere(Pos, SphereCastSize, 1 << 21).Length);
            Debug.Log("  Count: " + Physics.OverlapSphere(Pos, SphereCastSize, LayerMask.NameToLayer("Wall")).Length);
            if (Physics.OverlapSphere(Pos, SphereCastSize, LayerMask.NameToLayer("Wall")).Length == 0)
            {
                PhotonNetwork.Instantiate("Decal", Pos, Rot);
            }
        }
            
    }
    private void Update()
    {
        //MimicTester.instance.transform.positon;
    }
    public void SpawnDecalWall(Vector3 Pos, Vector3 WallNormal)
    {
        Quaternion Rot = Quaternion.LookRotation(new Vector3(1, 0, 0), WallNormal);
        SpawnDecalAtPosition(Pos, Rot);
        /*
        Vector3 MovebackPos = Pos + (WallNormal * Moveback);

        Physics.Raycast(MovebackPos, Rot * Vector3.forward, out RaycastHit hit, LayerMask.NameToLayer("Wall"));


        Vector3 RealWallCollidePoint = hit.point;

        Vector3 BackFromWall = RealWallCollidePoint + (DistanceToWall * (Rot * Vector3.forward));

        SpawnDecalAtPosition(After ? RealWallCollidePoint : MovebackPos, Rot);
        */
        // Calculate the new position 3 units away from the collider in the direction
        //Vector3 newPosition = hit.point + (Direction.normalized * DistanceToWall);


        // Do something with the new position...





    }
    public void SpawnDecalGround(Vector3 Pos)
    {
        
        
        Pos = new Vector3(Pos.x, FloorHeight, Pos.z);
        Quaternion Rot = Quaternion.Euler(new Vector3(90, 0, 0));
        SpawnDecalAtPosition(Pos, Rot);
    }
}
