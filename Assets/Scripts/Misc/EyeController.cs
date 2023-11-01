using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


namespace Eyes
{
    public enum Eyes
    {
        None = 0,
        Block = 1,
        Fire = 2,
    }
    public class EyeController : MonoBehaviour
    {
        public float ColorTime;
        private float Timer;
        public List<GameObject> AllEyes;
        public List<Material> EyeMats;

        public bool IsOfflineVariant;

        private void Start()
        {
            if (!IsOfflineVariant)
                return;



            Athena.Runtime.instance.SpellHolder.Spells[Spell.Fireball].SpellEvent += RecieveFireball;
            //Athena.Runtime.instance.SpellHolder.Spells[Spell.Flames].SpellEvent += RecieveFlames;
        }
        //each local controls rpcs for network as well
        public void RecieveFireball(Side side, int state)
        {
            if(state == 1)
            {
                SetEyes(Eyes.Fire);
            }
        }
        public void RecieveFlames(Side side, int state)
        {
            if (state == 1)
            {
                SetEyes(Eyes.Fire);
            }
        }




        [PunRPC]
        public void SetEyes(Eyes eyes)
        {
            //Set
            if (eyes != Eyes.None)
                Timer = ColorTime;
            for (var i = 0; i < AllEyes.Count; i++)
                AllEyes[i].GetComponent<SkinnedMeshRenderer>().material = EyeMats[(int)eyes];

            //check for is local and act
            if (IsOfflineVariant && NetworkPlayerSpawner.instance.SpawnedPlayerPrefab != null)
                NetworkPlayerSpawner.instance.SpawnedPlayerPrefab.GetPhotonView().RPC("SetEyes", RpcTarget.All, eyes);
        }

        // Update is called once per frame
        void Update()
        {
            if (Timer > 0)
                Timer -= Time.deltaTime;
            else
            {
                SetEyes(Eyes.None);
            }
        }
    }
}
