using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Photon.Pun;
using Photon.Realtime;
using static Odin.Net;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Aura : MonoBehaviourPunCallbacks
{
    public VisualEffect AuraVFX;
    public static int MaxHealth = 100;

    private PhotonView MyPhotonView;
    private void Start()
    {
        if (GetComponent<PhotonView>())
            MyPhotonView = GetComponent<PhotonView>();
    }
    public void OnHealthChange(int NewHealth)
    {
        Debug.Log("callked");
        float NormalizedValue = (float)NewHealth / (float)MaxHealth;
        AuraVFX.SetFloat("Health", NormalizedValue);
        if (NewHealth == 0)
        {
            AuraVFX.Stop();
        }
        else if (NewHealth == MaxHealth)
        {
            Debug.Log("stat");
            AuraVFX.Play();
        }
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey(ID.PlayerHealth))
        {
            //Debug.Log("" + (int)changedProps[ID.PlayerHealth]);
            if (MyPhotonView != null && !targetPlayer.IsLocal)
            {
                if (MyPhotonView.Owner == targetPlayer)
                {
                    OnHealthChange((int)changedProps[ID.PlayerHealth]);
                    return;
                }
                    
            }
            if (targetPlayer.IsLocal && MyPhotonView == null)
            {
                
                OnHealthChange((int)changedProps[ID.PlayerHealth]);
                return;
            }

            AuraVFX.Stop();

        }
    }
}
