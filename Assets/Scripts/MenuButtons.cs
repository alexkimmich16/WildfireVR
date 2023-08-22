using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Sirenix.OdinInspector;
namespace Menu
{
    public enum MenuButtonType
    {
        Resume = 0,
        Options = 1,
        Quit = 2,
    }
    [RequireComponent(typeof(VFXText))]
    public class MenuButtons : SerializedMonoBehaviour
    {
        public Menu menuType;
        [ShowIf("menuType", Menu.Base)] public MenuButtonType type;

        [ShowIf("menuType", Menu.Options)] public bool Increase;

        private float Timer;

        //private float CooldownTimer;
        private void OnTriggerEnter(Collider other)
        {
            if (!MenuController.instance.CanPress)
                return;

            if (LayerMask.LayerToName(other.gameObject.layer) == "Hand")
            {
                if (menuType == Menu.Base)
                {
                    MenuController.instance.ButtonTouched(type);
                    GetComponent<VFXText>().SetVFXColor(true);
                }
                else if(menuType == Menu.Options)
                {
                    if (GetComponent<VFXText>().displayType == OptionsButtons.VolumeButton)
                    {
                        MenuEffect.instance.ButtonTouched(GetComponent<VFXText>().SoundType, Increase);
                        GetComponent<VFXText>().SetVFXColor(true);

                        Timer = MenuEffect.instance.ButtonColorTime;
                    }
                    

                    else if (GetComponent<VFXText>().displayType == OptionsButtons.BackButton)
                    {
                        
                        MenuController.instance.OptionsBackButton();

                        GetComponent<VFXText>().SetVFXColor(true);
                        Timer = MenuEffect.instance.ButtonColorTime;
                    }
                    else if (GetComponent<VFXText>().displayType == OptionsButtons.QualityChange)
                    {
                        MenuEffect.instance.QualityChange(Increase);

                        GetComponent<VFXText>().SetVFXColor(true);
                        Timer = MenuEffect.instance.ButtonColorTime;
                    }

                }
                
            }
                
        }
        public void ResetTimer()
        {
            //CooldownTimer = MenuController.instance.NewMenuTouchCooldown;
        }
        private void Update()
        {

            //CooldownTimer -= Time.deltaTime;

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

