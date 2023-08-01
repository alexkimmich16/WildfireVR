using UnityEngine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using static Odin.Net;
using ExitGames.Client.Photon;
namespace Skin
{
    public class PlayerSkin : MonoBehaviourPun
    {
        private SkinController SC => SkinController.instance;

        public SkinnedMeshRenderer TorsoAndArms;
        public SkinnedMeshRenderer Pants;
        public SkinnedMeshRenderer Hood;
        public SkinnedMeshRenderer Belt;

        public int SkinNum;
        private Material ActiveBodyMat;
        private Material ActiveHoodMat;
        public bool ManualTest;
        private bool GivenLocalSkin;

        private void Start()
        {
            ActiveBodyMat = new Material(TorsoAndArms.materials[1]);
            ActiveHoodMat = new Material(Hood.materials[0]);

            ActiveBodyMat.name = "ActiveBody";
            ActiveHoodMat.name = "ActiveHood";
            TorsoAndArms.sharedMaterials = new Material[] { ActiveBodyMat, ActiveBodyMat, TorsoAndArms.sharedMaterials[2] };
            Pants.sharedMaterials = new Material[] { ActiveBodyMat, ActiveBodyMat };
            Hood.sharedMaterials = new Material[] { ActiveHoodMat };
            Belt.sharedMaterials = new Material[] { ActiveBodyMat };

            
        }
        private void Update()
        {
            
            if (ManualTest)
            {
                ChangeSkin(SkinNum);


                return;
            }

            if (photonView == null)
                return;

            if (!GivenLocalSkin && Exists(ID.PlayerTeam, photonView.Owner))
            {
                GivenLocalSkin = true;
                int NewTeam = (int)GetPlayerTeam(photonView.Owner);
                ChangeSkin(NewTeam);

                if(photonView.Owner == PhotonNetwork.LocalPlayer)
                {
                    SkinController.instance.MyRigSkin.ChangeSkin(NewTeam);
                }
            }
        }
        public void ChangeSkin(int skinIndex)
        {
            if (skinIndex >= 0 && skinIndex < SC.Colors.Count)
            {
                //Debug.Log("ChangedSkin");
                ActiveBodyMat.color = SC.Colors[skinIndex].BodyColor;
                ActiveHoodMat.color = SC.Colors[skinIndex].HoodColor;
            }
        }

        public void RaiseSkinChangeEvent(int viewId, int skinIndex)
        {
            // Set up the event data
            object[] content = new object[] { viewId, skinIndex };

            // Raise the event
            PhotonNetwork.RaiseEvent(SkinController.SkinChangeCode, content, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
        }

        

        
    }
}

