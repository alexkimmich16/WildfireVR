using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.SDF;
using TMPro;
using static Odin.Net;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine.XR;
namespace Menu
{
    public class MenuController : SerializedMonoBehaviour
    {
        public static MenuController instance;
        void Awake() { instance = this; }

        public delegate void MenuChange(bool NewState);
        public static event MenuChange OnMenuState;
        public static event MenuChange OnOptionsState;

        public int VolumeChange;

        //on button press enable/disable menu
        //support resume, options, quit, etc
        private bool LastButton;
        private bool MenuOpen;
        private bool OptionsOpen;




        [Button]public void OpenButton() { SetMenu(true); }
        [Button] public void CloseButton() { SetMenu(false); }
        [Button] public void OpenOptionsButton() { SetOptionsMenu(true); }
        [Button] public void CloseOptionsButton() { SetOptionsMenu(false); }

        private void Update()
        {
            InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.menuButton, out bool ButtonDown);
            bool ButtonPressed = ButtonDown == true && LastButton == false;

            if (ButtonPressed)
                OnMenuToggle();
            LastButton = ButtonDown;
        }

        public void OnMenuToggle()
        {
            if (OptionsOpen || MenuOpen)
            {
                SetMenu(false);
                SetOptionsMenu(false);
            }
            else if (!OptionsOpen && !MenuOpen)
            {
                SetMenu(true);
            }
        }

        public void SetMenu(bool State)
        {
            //Debug.Log("MenuState: " + State);
            MenuOpen = State;
            OnMenuState?.Invoke(State);
        }

        public void SetOptionsMenu(bool State)
        {
            OptionsOpen = State;
            OnOptionsState?.Invoke(State);
        }

        //day to day schedualing
        //pt time

        public void ChangeVolume()
        {
            //VolumeChange
        }
    }
}

