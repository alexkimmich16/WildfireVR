using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class DatabaseManage : MonoBehaviour
{
    public static DatabaseManage instance;
    void Awake() { instance = this; }

    private const string ServerUrl = "https://arcane-citadel-75129.herokuapp.com/"; // Replace with your Heroku app URL
    public void GetData()
    {
        StartCoroutine(GetDataCoroutine());
    }

    private IEnumerator GetDataCoroutine()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(ServerUrl + "/api/data"))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
            }
            else
            {
                string response = request.downloadHandler.text;
                Debug.Log(response);
                // Process the response data here
            }
        }
    }
}
