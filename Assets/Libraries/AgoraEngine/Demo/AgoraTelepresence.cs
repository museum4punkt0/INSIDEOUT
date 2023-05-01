using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if(UNITY_2018_3_OR_NEWER) 
using UnityEngine.Android;
#endif

public class AgoraTelepresence : MonoBehaviour
{
    void Start()
    {
#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
		permissionList.Add(Permission.Microphone);         
		permissionList.Add(Permission.Camera);               
#endif
    }
    private void CheckPermission()
    {
#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
        foreach(string permission in permissionList)
        {
            if (Permission.HasUserAuthorizedPermission(permission))
            {
            }
            else
            {
                Permission.RequestUserPermission(permission);
            }
        }
#endif
    }
    void Update()
    {
#if (UNITY_2018_3_OR_NEWER)
        // Ask for your Android device's permissions.
        CheckPermission();
#endif
    }
}
