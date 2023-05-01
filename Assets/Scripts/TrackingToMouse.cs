//Manages the mouse of the user (enabling/disabling when the hand gets farer or closer to the screen, hand position in general, clicking buttons and the related animation)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrackingToMouse : MonoBehaviour
{
    
    [SerializeField]
    private float smooth;
    public Transform target;
    public Camera cam;
    public RectTransform mouse;
    public GameObject Mouse;
    public Button[] Buttons;
    public Animator AnimButton;
    public GameObject Drawing;
    public GameObject DescriberActive;
    public string ChoosenHand = "WristRight";

    float timer= 1.5f;
    float timerMax= 1.5f;

    bool newTarget = false;
    bool allButtonsOff = false;


    void Start()
    {
        AnimationClip[] clips = AnimButton.runtimeAnimatorController.animationClips;
        timerMax = clips[0].length;
    }
    

    void Update()
    {
        //If no target make new hand position
        if(!target || newTarget == true)
        {
            if(GameObject.Find(ChoosenHand)){
                target = GameObject.Find(ChoosenHand).transform;
                newTarget = false;
                //Debug.Log("target is"+target.name);
            }
        }
        //Else get wrist position and manages different modes
        else
        {
            Vector3 screenPos = cam.WorldToScreenPoint(target.position);
            //Debug.Log("target is " + screenPos.x + " pixels from the left"+ screenPos.y);
            Vector2 wirstPosition = new Vector2(screenPos.x, screenPos.y);
            if(wirstPosition.y>Screen.height)
            wirstPosition.y=Screen.height;
            if(wirstPosition.y<0)
            wirstPosition.y=0;
            if(wirstPosition.x>Screen.width)
            wirstPosition.x=Screen.width;
            if(wirstPosition.x<0)
            wirstPosition.x=0;

            //When draing is activated
            if (Drawing.activeSelf)
            {
                DrawingScript drawingScript = Drawing.GetComponent<DrawingScript>();
                //If hand is far enough away from the screen activate mouse and buttons
                if (drawingScript.handLerpPos3.z > drawingScript.distanceDrawing)
                {
                    Mouse.SetActive(true);
                    mouse.anchoredPosition = Vector2.Lerp(mouse.anchoredPosition, wirstPosition, smooth);

                    //If all buttons are displayed as inactive, display all as active
                    if (allButtonsOff == true)
                    {
                        for (int b = 0; b < Buttons.Length; b++)
                        {
                            Buttons[b].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1);
                        }
                        allButtonsOff = false;
                    }
                    Click();
                }
                //Else deactivate mouse and buttons
                else
                {
                    mouse.anchoredPosition = Vector2.Lerp(mouse.anchoredPosition, wirstPosition, smooth);
                    Mouse.SetActive(false);

                    //Display all buttons as inactive
                    for (int b = 0; b < Buttons.Length; b++)
                    {
                        Buttons[b].GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f, 1);
                    }
                    allButtonsOff = true;
                }
            }
            //When in Pantomime, Describer mode, deactivate mouse because there are no buttons in this mode
            else if (DescriberActive.activeSelf && !Drawing.activeSelf)
            {
                Mouse.SetActive(false);
            }
            //Else activate mouse and buttons
            else
            {
                Mouse.SetActive(true);
                mouse.anchoredPosition = Vector2.Lerp(mouse.anchoredPosition, wirstPosition, smooth);

                //If all buttons are displayed as inactive, display all as active
                if (allButtonsOff == true)
                {
                    for (int b = 0; b < Buttons.Length; b++)
                    {
                        Buttons[b].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1);
                    }
                    allButtonsOff = false;
                }
                Click();
            }
        }
    }

    //Tracks button position in relation to the hand position and marks every other button as in active if the hand is on a specific button
    void Click()
    {
        bool setClick = false;
        for(int i=0; i < Buttons.Length;i++){
            if(Buttons[i].interactable&&Buttons[i].IsActive()){
                RectTransform bttn_Rec = Buttons[i].GetComponent<RectTransform>();
                if (mouse.anchoredPosition.x > bttn_Rec.anchoredPosition.x - bttn_Rec.sizeDelta.x / 2 && mouse.anchoredPosition.x < bttn_Rec.anchoredPosition.x + bttn_Rec.sizeDelta.x / 2 && mouse.anchoredPosition.y > bttn_Rec.anchoredPosition.y - bttn_Rec.sizeDelta.y / 2 && mouse.anchoredPosition.y < bttn_Rec.anchoredPosition.y + bttn_Rec.sizeDelta.y / 2)
                {
                    for (int x = 0; x < Buttons.Length; x++)
                    {
                        if (x == i)
                        {
                            Buttons[x].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1);
                        }
                        else
                        {
                            Buttons[x].GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f, 1);
                        }
                    }

                    SetClick(i);
                    setClick = true;
                }
            }
        }
        //Make all buttons active and reset animation if hand is on no button
        if(!setClick)
        {
            for (int x = 0; x < Buttons.Length; x++)
            {
                Buttons[x].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1);
            }
            mouse.localScale = new Vector3(1,1,1);
            AnimButton.SetBool("Anim", false);
            timer = 0;
        }
        
    }

    //Click on button after animation
    void SetClick(int i)
    {
        mouse.localScale = new Vector3(2,2,1);
        AnimButton.SetBool("Anim", true);
        timer += Time.deltaTime;
        if(timer>timerMax)
        {
            Debug.Log("Invoke: " +i + "  "+ timer);
            Buttons[i].onClick.Invoke();
            timer = 0;
        }
    }

    //Set right hand for mouse
    public void RightHand()
    {
        ChoosenHand = "WristRight";
        newTarget = true;
    }

    //Set left hand for mouse
    public void LeftHand()
    {
        ChoosenHand = "WristLeft";
        newTarget = true;
    }
}