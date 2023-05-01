// Source:https://www.youtube.com/watch?v=HbXP3sSx9vE


using agora_gaming_rtc;
using DilmerGames.Core.Singletons;
using UnityEngine;
using UnityEngine.UI;

public class AgoraUnityVideo : Singleton<AgoraUnityVideo>
{

    private IRtcEngine mRtcEngine;
    private string token;
    private int lastError;
    private uint localUserId;

    public uint LocalUserId
    {
        get
        {
            return localUserId;
        }
    }

    public void LoadEngine(string appId, string token = null)
    {
        //Logger.Instance.LogInfo("Loading Engine initialization");

        this.token = token;

        if (mRtcEngine != null)
        {
            //Logger.Instance.LogInfo("Engine exist. Please unload it first!");
            return;
        }

        mRtcEngine = IRtcEngine.getEngine(appId);
        mRtcEngine.SetLogFilter(LOG_FILTER.DEBUG | LOG_FILTER.INFO | LOG_FILTER.WARNING | LOG_FILTER.ERROR | LOG_FILTER.CRITICAL);
    }

    public void Join(string channel)
    {
        //Logger.Instance.LogInfo($"Calling join channel {channel}");

        if (mRtcEngine == null) return;

        mRtcEngine.OnJoinChannelSuccess = OnJoinChannelSuccess;
        mRtcEngine.OnUserJoined = OnUserJoined;
        mRtcEngine.OnUserOffline = OnUserOffline;
        mRtcEngine.OnWarning = (int warn, string msg) =>
        {
            //Logger.Instance.LogInfo($"Warning code: {warn} with message: {msg}");
        };

        mRtcEngine.OnError = HandleError;
        mRtcEngine.EnableVideo();

        mRtcEngine.EnableVideoObserver();

        mRtcEngine.JoinChannelByKey(channelKey: token, channelName: channel);
    }

    public void Leave()
    {
        //Logger.Instance.LogInfo($"Leaving channel");
        if (mRtcEngine == null) return;
        mRtcEngine.LeaveChannel();
        mRtcEngine.DisableVideoObserver();
        GameObject go = GameObject.Find($"{localUserId}");
        if (go != null) Destroy(go);
    }

    public void UnloadEngine()
    {
        //Logger.Instance.LogInfo($"Calling UnloadEngine");

        if (mRtcEngine != null)
        {
            IRtcEngine.Destroy();
            mRtcEngine = null;
        }
    }

    public void EnableVideo(bool pauseVideo)
    {
        if(mRtcEngine != null)
        {
            if(!pauseVideo)
            {
                mRtcEngine.EnableVideo();
            }
            else
            {
                mRtcEngine.DisableVideo();
            }
        }
    }

    public void OnJoinChannelSuccess(string channelName, uint uid, int elapsed)
    {
        localUserId = uid;
        //Logger.Instance.LogInfo($"OnJoinChannelSuccess: uid {uid}");
        //Logger.Instance.LogInfo($"SDK Version: {IRtcEngine.GetSdkVersion()}");

        GameObject childVideo = GetChildVideoLocation(uid);
        MakeImageVideoSurface(childVideo);
    }

    public void OnUserJoined(uint uid, int elapsed)
    {
        //Logger.Instance.LogInfo($"OnUserJoined {uid}");

        GameObject childVideo = GetChildVideoLocation(uid);

        VideoSurface videoSurface = MakeImageVideoSurface(childVideo);

        if(videoSurface != null)
        {
            videoSurface.SetForUser(uid);
            videoSurface.SetEnable(true);
            videoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
        }
    }

    public void OnUserOffline(uint uid, USER_OFFLINE_REASON reason)
    {
        //Logger.Instance.LogInfo($"OnUserOffline {uid}");
        GameObject go = GameObject.Find($"{uid}");
        if (go != null) Destroy(go);
    }

    public void HandleError(int error, string msg)
    {
        if (error == lastError) return;

        //Logger.Instance.LogError($"Error code generated {error}");
        //Logger.Instance.LogError($"Error message generated {msg}");

        lastError = error;
    }

    private GameObject GetChildVideoLocation(uint uid)
    {
        GameObject go = GameObject.Find("Videos");
        GameObject childVideo = go.transform.Find($"{uid}")?.gameObject;

        if (childVideo == null)
        {
            childVideo = new GameObject($"{uid}");
            childVideo.transform.parent = go.transform;
        }
        return childVideo;
    }

    private VideoSurface MakeImageVideoSurface(GameObject go)
    {
        go.AddComponent<RawImage>();
        go.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        var rectTransform = go.GetComponent<RectTransform>();

        rectTransform.sizeDelta = new Vector2(60.0f, 50.0f);
        rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, rectTransform.localPosition.y, 0);

        rectTransform.localRotation = new Quaternion(0, rectTransform.localRotation.y, -180.0f, rectTransform.localRotation.w);

        return go.AddComponent<VideoSurface>();
    }
}
