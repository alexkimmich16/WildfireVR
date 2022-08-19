using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
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
