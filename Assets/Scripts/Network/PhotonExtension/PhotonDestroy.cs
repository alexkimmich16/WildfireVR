using UnityEngine;
using Photon.Pun;
public class PhotonDestroy : MonoBehaviour
{
   
    public float LifeTime;
    public float Timer;
    private bool CountdownIsActive;
    public bool StartCountdownOnStart = false;
    public void StartCountdown()
    {
        CountdownIsActive = true;
    }
    [PunRPC]
    public void DestroyOnline()
    {
        if (GetComponent<PhotonView>().IsMine)
            PhotonNetwork.Destroy(gameObject);
    }
    void Update()
    {
        if(transform.position == Vector3.zero)
            gameObject.SetActive(false);
        
        if (!CountdownIsActive)
            return;
        Timer += Time.deltaTime;
        if (Timer > LifeTime)
            if (GetComponent<PhotonView>() != null && GetComponent<PhotonView>().IsMine)
                DestroyOnline();
    }
    private void OnDisable()
    {
        Timer = 0f;
        CountdownIsActive = false;
    }
    private void OnEnable()
    {
        if(StartCountdownOnStart)
            CountdownIsActive = true;
    }
}