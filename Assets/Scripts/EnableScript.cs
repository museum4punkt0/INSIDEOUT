// This script enables or disables the drawing for the different panales of the tutorial
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableScript : MonoBehaviour
{
    public GameObject DrawingObject;
    public bool activateDrawing = true;

    public GameObject DescriberObject;


    void Update()
    {
        if (this.gameObject.activeSelf == true && activateDrawing == true)
        {
            DrawingObject.SetActive(true);
        }
        else if (this.gameObject.activeSelf == true && activateDrawing == false)
        {
            DrawingObject.SetActive(false);
            DescriberObject.SetActive(false);
        }
    }
}
