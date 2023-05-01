using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DilmerGames.Core.Singletons;
#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
using UnityEngine.Android;
#endif

public class AgoraVideoSetup : Singleton<AgoraVideoSetup>
{
    public enum ChannelActions
    {
        JOIN,
        LEAVE
    }

    [SerializeField]
    private Button joinChannelButton;

    [SerializeField]
    private string appId = "f5aa6d8722db4e24986a6e2724e95fb8";

    [SerializeField]
    private string channelName = "TP";

    [SerializeField]
    private string token = "your_token";

    private bool settingsReady;

    private TextMeshProUGUI joinChannelButtonText;

    private Image joinChannelButtonImage;

#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
    private ArrayList permissionList = new ArrayList();
#endif

    private void Awake()
    {
        joinChannelButtonText = joinChannelButton.GetComponentInChildren<TextMeshProUGUI>();

        joinChannelButtonImage = joinChannelButton.GetComponent<Image>();
        joinChannelButtonImage.color = Color.green;

#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
    permissionList.Add(Permission.Microphone);
    permissionList.Add(Permission.Camera);
#endif

        DontDestroyOnLoad(gameObject);

    }

    private void Start()
    {
        if (string.IsNullOrEmpty(appId) || string.IsNullOrEmpty(channelName))
        {
            settingsReady = false;
        }
        else
        {
            settingsReady = true;
        }

        joinChannelButton.onClick.AddListener(() =>
        {
            if (joinChannelButtonText.text.Contains($"{ChannelActions.JOIN}"))
            {
                StartAgora();
            }
            else
            {
                LeaveAgora();
            }
        });
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.S)) StartAgora();
        if (Input.GetKeyDown(KeyCode.L)) LeaveAgora();
#endif
    }

    public uint GetAgoraUserId() => AgoraUnityVideo.Instance.LocalUserId;

    public void StartAgora()
    {
        if (settingsReady)
        {
            CheckPermissions();
            AgoraUnityVideo.Instance.LoadEngine(appId, token);
            AgoraUnityVideo.Instance.Join(channelName);

            joinChannelButtonText.text = $"{ChannelActions.LEAVE} CHANNEL";
            joinChannelButtonImage.color = Color.yellow;
        }
    }

    public void LeaveAgora()
    {
        AgoraUnityVideo.Instance.Leave();
        joinChannelButtonText.text = $"{ChannelActions.JOIN} CHANNEL";
        joinChannelButtonImage.color = Color.white;
    }

    private void OnApplicationPause(bool pause)
    {
        AgoraUnityVideo.Instance.EnableVideo(pause);
    }

    private void OnApplicationQuit()
    {
        AgoraUnityVideo.Instance.UnloadEngine();
    }

    private void CheckPermissions()
    {
#if (UNITY_2018_3_OR_NEWER && UNITY_ANDRAID)
        foreach (string permission in permissionList)
        {
            if (!Permission.HasUserAuthorizedPermission(permission))
            {
                Permission.RequestUserPermission(permission);
            }
        }
#endif
    }
}