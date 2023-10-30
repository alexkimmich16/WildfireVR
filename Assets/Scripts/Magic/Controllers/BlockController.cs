using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BlockController : SpellControlClass
{
    public static BlockController instance;
    void Awake() { instance = this; }
    private static Dictionary<Side, bool> ActiveParries = new  Dictionary<Side, bool> {{Side.right, false}, { Side.left, false } };

    public float MaxParryDistance = 2f;
    //could be:
    //parry, wait for false frame
    //parry every frame active
    public override void RecieveNewState(Side side, int State)
    {
        if (InGameManager.instance.CanDoMagic == false)
            return;

        ActiveParries[side] = State == 1;
        if (State == 1)
        {
            //Parry
            Side OtherSide = side == Side.right ? Side.left : Side.right;
            if(ActiveParries[OtherSide] == true)
            {
                Parry(side);
                //DoubleParry();
            }
            else
            {
                Parry(side);
            }

            

        }
    }

    public void Parry(Side side)
    {
        foreach(FireballObject fireball in FireballController.instance.Fireballs)
        {
            if(Vector3.Distance(fireball.transform.position, AIMagicControl.instance.Cam.position) < MaxParryDistance)
            {
                fireball.GetComponent<PhotonView>().RPC("Parried", RpcTarget.All);
                //fireball.Parried();
            }
        }
        
        
        //check for fireballs nearby
    }
    public void DoubleParry()
    {

    }

    public override void InitializeSpells()
    {
        //BlockVFXObject = PhotonNetwork.Instantiate(AIMagicControl.instance.spells.SpellName(Spell.SideParry, 0), Vector3.zero, Quaternion.identity);
        //BlockVFXObject.GetComponent<PhotonView>().RPC("SetOnlineVFX", RpcTarget.AllBuffered, false);
    }
}
