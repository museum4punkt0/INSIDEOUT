using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using com.rfilkov.kinect;

public class AgoraLocalRender : MonoBehaviour
{
    //public GameObject LocalUserPlane; // If plane is needed instead of canvas
    public RawImage LocalUser;
    public int sensorIndex = 0;

    private KinectManager kinectManager = null;
    private KinectInterop.SensorData sensorData = null;

    //Renderer LocalUserRenderer;
    Texture clrTex;

    void Start()
    {
        // Get kinectManager and sensorData
        kinectManager = KinectManager.Instance;
        sensorData = kinectManager != null ? kinectManager.GetSensorData(sensorIndex) : null;
    }

    void Update()
    {
        if (kinectManager && kinectManager.IsInitialized())
        {
            // Get color images from kinect and make texture
            clrTex = kinectManager.GetColorImageTex(sensorIndex);
            LocalUser.texture = clrTex;

             /* For plane:
            LocalUserRenderer = LocalUserPlane.GetComponent<Renderer>();
            LocalUserRenderer.material.mainTexture = clrTex;
             */
        }
    }
}
