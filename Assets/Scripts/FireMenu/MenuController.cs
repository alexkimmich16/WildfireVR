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
    public enum Menu
    {
        Base = 0,
        Options = 1,
    }
    public class MenuController : SerializedMonoBehaviour
    {
        public static MenuController instance;
        void Awake() { instance = this; }

        public delegate void MenuChange(bool NewState);
        public static event MenuChange OnMenuState;
        public static event MenuChange OnOptionsState;

        //public static Dictionary<Menu, MenuChange> MenuStateEvents = new Dictionary<Menu, MenuChange>() { {Menu.Base, OnMenuState},{Menu.Options, OnOptionsState}};
        public static List<MenuChange> MenuStateEvents = new List<MenuChange>() { OnMenuState, OnOptionsState };
        public static MenuChange menuEvent(int Index)
        {
            if (Index == 0)
                return OnMenuState;
            else
                return OnOptionsState;
        }

        public const float VolumeChange = 0.1f;
        

        //on button press enable/disable menu
        //support resume, options, quit, etc
        private bool LastButton;
        private bool MenuOpen;
        private bool OptionsOpen;


        public float NewMenuTouchCooldown = 1f;
        private float PressCooldownTimer;
        public bool CanPress { get { return PressCooldownTimer <= 0f; } }

        public bool AnyMenusOpen { get { return MenuOpen || OptionsOpen; } }




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

            PressCooldownTimer -= Time.deltaTime;
        }
        private void Start()
        {
            SetMenu(false);
            SetOptionsMenu(false);
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
            PressCooldownTimer = NewMenuTouchCooldown;
            //Debug.Log("MenuState: " + State);
            MenuOpen = State;
            OnMenuState?.Invoke(State);
        }

        public void SetOptionsMenu(bool State)
        {
            PressCooldownTimer = NewMenuTouchCooldown;
            //Debug.Log("OptionsState: " + State);
            OptionsOpen = State;
            OnOptionsState?.Invoke(State);
        }

        public void OptionsBackButton()
        {
            SetOptionsMenu(false);
            SetMenu(true);
        }

        public void ButtonTouched(MenuButtonType type)
        {
            SetMenu(false);
            //Debug.Log("" + other.gameObject.name);
            if (type == MenuButtonType.Resume)
            {
                SetOptionsMenu(false);

            }
            else if (type == MenuButtonType.Options)
            {
                SetOptionsMenu(true);
                //open options menu
            }
            else if (type == MenuButtonType.Quit)
            {
                SetOptionsMenu(false);
                //bring up are you sure menu
            }
        }
    }
}

