using UnityEngine;
using Valve.VR;
using UnityEngine.UI;
public class CameraTest : MonoBehaviour
{
    /*
    public SteamVR_TrackedCamera.VideoStreamTexture videoStreamTexture;
    private uint deviceIndex;
    public GameObject cameraDisplay; // Assign the GameObject in the Inspector

    public Texture2D TextureMap;
    public Image image;
    private Sprite mySprite;

    public int Source;
    public bool Undistorted;

    
    private void Start()
    {
        //deviceIndex = SteamVR_TrackedCamera.(SteamVR_TrackedCamera.DeviceRelation.Closest);
        //deviceIndex = SteamVR_TrackedCamera.GetDeviceIndex(SteamVR_TrackedCamera.DeviceRelation.Closest);
        //videoStreamTexture = SteamVR_TrackedCamera.Source(true, 0);
        //videoStreamTexture = SteamVR_TrackedCamera.;
    }

    private void Update()
    {
        videoStreamTexture = SteamVR_TrackedCamera.Source(Undistorted, Source);
        videoStreamTexture.Acquire();
        //videoStreamTexture.
        Debug.Log("1");

        //TextureMap = videoStreamTexture.texture;
        SetImage(videoStreamTexture.texture);

        if (videoStreamTexture.hasCamera)
        {
            Debug.Log("2");
            
            // Assuming cameraDisplay has a Renderer component
            //cameraDisplay.GetComponent<Renderer>().material.mainTexture = TextureMap;
            
        }
        videoStreamTexture.Release();

        //if (SteamVR_TrackedCamera.HasVideoStream(deviceIndex))
        //{
            
        //}
    }
    

    public void SetImage(Texture2D tex)
    {
        mySprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 1000.0f);
        image.sprite = mySprite;

    }
    */
}
