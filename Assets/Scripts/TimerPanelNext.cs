// Timer script for the first two tutorial panels, so that the next panel appears after a specific amount of time
// Is used before the user is described how to control the UI
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerPanelNext : MonoBehaviour
{
    public float switchTime = 6.0f;
    private float panelTime = 0.0f;
    public ChangeModus CM;

    void Update()
    {
        if (this.gameObject.activeSelf)
        {
            panelTime = panelTime + Time.deltaTime;
            if (panelTime >= switchTime)
            {
                CM.NextCount(0);
                panelTime = 0.0f;
            }
        }
    }
}