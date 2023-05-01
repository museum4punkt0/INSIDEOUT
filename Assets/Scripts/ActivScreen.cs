using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivScreen : MonoBehaviour
{
    void Start()
    {
        // Adjusts the size of the application to the screen
        Display.displays[0].Activate(Display.displays[0].systemWidth, Display.displays[0].systemHeight, 60);
    }
}
