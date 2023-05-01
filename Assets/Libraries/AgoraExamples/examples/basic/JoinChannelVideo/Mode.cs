using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mode : MonoBehaviour
{
    string messageStr;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(messageStr != SendStreamMessageSample.messagestring)
        {
            messageStr = SendStreamMessageSample.messagestring;
            int modeInt = 0;
            int.TryParse(messageStr, out modeInt);
            Debug.Log(modeInt);
        }
    }
}
