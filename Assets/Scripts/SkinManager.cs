using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Data
{
    public class SkinManager : MonoBehaviour
    {
        // Start is called before the first frame update

        public List<SkinColors> Skins = new List<SkinColors>();
        public void SetMeshColor(int Skin)
        {

        }

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public struct SkinColors
        {
            public string Name;
            public Color Skin;
            public Color Body;
            public Color Hair;
            public Color Hood;
        }
    }
}

