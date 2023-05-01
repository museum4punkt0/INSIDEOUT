// Timer script for the first two tutorial panels, so that the next panel appears after a specific amount of time
// Is used before the user is described how to control the UI
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerPanel : MonoBehaviour
{
    public float switchTime = 6.0f;
    private float panelTime = 0.0f;
    public GameObject TutorialPanel1;
    public GameObject TutorialPanel2;

    void Update()
    {
        if (this.gameObject.activeSelf)
        {
            panelTime = panelTime + Time.deltaTime;
            if (panelTime >= switchTime)
            {
                TutorialPanel1.SetActive(false);
                TutorialPanel2.SetActive(true);
                panelTime = 0.0f;
            }
        }
    }
}