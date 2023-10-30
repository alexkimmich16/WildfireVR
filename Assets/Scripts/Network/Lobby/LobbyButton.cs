using UnityEngine;
using TMPro;
namespace Lobby
{
    public class LobbyButton : MonoBehaviour
    {
        public TextMeshProUGUI RoomName;
        private LobbyManager manager;
        private void Start()
        {
            manager = LobbyManager.instance;
        }
        public void SetName(string Name)
        {
            RoomName.text = Name;
        }
        public void OnButtonPress()
        {
            manager.JoinRoom(RoomName.text);
        }
    }
}

