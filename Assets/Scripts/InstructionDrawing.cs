// Script that manages the drawing points tutorial panel and automatically gues to next panel if all three points are selected while drawing
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InstructionDrawing : MonoBehaviour
{

    public GameObject[] images;
    bool[] pointDrawn = new bool[3];
    public TrackingToMouse tTM;
    public DrawingScript dS;
    public GameObject PanelDrawingA;
    public GameObject PanelDrawingB;
    public ChangeModus CM;
    public LineAnimator LA1;
    public LineAnimator LA2;
    public LineAnimator LA3;

    void Start()
    {
        pointDrawn[0]=false;
        pointDrawn[1]=false;
        pointDrawn[2]=false;
    }


    void Update()
    {
        //If hand position is on point position while drawing disable point and mark that it was drawn on
        for(int i=0; i < images.Length;i++){
            RectTransform bttn_Rec = images[i].GetComponent<RectTransform>();
            Debug.Log(bttn_Rec);
            if(tTM.mouse.anchoredPosition.x > bttn_Rec.anchoredPosition.x-bttn_Rec.sizeDelta.x/2 && tTM.mouse.anchoredPosition.x < bttn_Rec.anchoredPosition.x+bttn_Rec.sizeDelta.x/2 && tTM.mouse.anchoredPosition.y > bttn_Rec.anchoredPosition.y-bttn_Rec.sizeDelta.y/2 && tTM.mouse.anchoredPosition.y < bttn_Rec.anchoredPosition.y+bttn_Rec.sizeDelta.y/2)
            {
                if(dS.handLerpPos3.z < dS.distanceDrawing)
                {
                    pointDrawn[i]=true;
                    images[i].SetActive(false);
                }
            }
        }
        //If all points are selected go to next panel
        if(pointDrawn[0]==true && pointDrawn[1]==true && pointDrawn[2]==true)
        {
            CM.ClearDrawing();
            PanelDrawingA.SetActive(false);
            PanelDrawingB.SetActive(true);
            ResetPoints();
            LA1.ResetLineAnimation();
            LA2.ResetLineAnimation();
            LA3.ResetLineAnimation();
        }
    }

    public bool CheckPoints()
    {
        if (pointDrawn[0] == true || pointDrawn[1] == true || pointDrawn[2] == true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ResetPoints()
    {
        pointDrawn[0] = false;
        pointDrawn[1] = false;
        pointDrawn[2] = false;
        images[0].SetActive(true);
        images[1].SetActive(true);
        images[2].SetActive(true);
    }
}
