using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Sirenix.OdinInspector;
namespace Menu
{
    public enum MenuType
    {
        Resume = 0,
        Options = 1,
        Quit = 2,
    }

    public class MenuButtons : SerializedMonoBehaviour
    {
        public Menu buttonType;
        [ShowIf("buttonType", Menu.Base)] public MenuType type;

        [ShowIf("buttonType", Menu.Options)] public bool Increase;
        [ShowIf("buttonType", Menu.Options)] public SoundType SoundType;

        private float Timer;
        private void OnTriggerEnter(Collider other)
        {
            if (LayerMask.LayerToName(other.gameObject.layer) == "Hand")
            {
                if (buttonType == Menu.Base)
                {
                    MenuEffect.instance.ButtonTouched(type);
                    GetComponent<VFXText>().SetVFXColor(true);
                }
                else if(buttonType == Menu.Options)
                {
                    MenuEffect.instance.ButtonTouched(SoundType, Increase);
                    GetComponent<VFXText>().SetVFXColor(true);

                    Timer = MenuEffect.instance.ButtonColorTime;
                }
                
            }
                
        }

        private void Update()
        {
            if (Timer > 0)
                Timer -= Time.deltaTime;
            if (Timer < 0)
            {
                Timer = 0f;
                GetComponent<VFXText>().SetVFXColor(false);
            }
        }
    }
}

