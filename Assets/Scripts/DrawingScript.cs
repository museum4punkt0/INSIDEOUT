// This script does the whole drawing managment
/// <Summary>
/// <href="https://www.youtube.com/watch?v=_ILOVprdq4">Source</see>
/// </Summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using com.rfilkov.kinect;

public class DrawingScript : MonoBehaviour
{
    public GameObject plane;
    public GameObject brush;
    public float smooth;
    public float distanceDrawing;
    public float planeDistance;

    public string ChoosenHandDrawing = "WristRight";

    private GameObject hand;
    private Vector3 handPos3;
    public Vector3 handLerpPos3;
    private Vector2 handLerpPos2;
    private bool firstHand = true;

    LineRenderer currentLineRenderer;

    Vector2 lastPos;
    Vector2 handPos;
    bool drawingPause = true;

    private void Update()
    {   
        //Find hand of the user -> default right hand
        hand = GameObject.Find("SkeletonCollider/"+ChoosenHandDrawing);
        
        //If found hand start script
        if (hand)
        {
            Drawing();
        }
    }

    void Drawing()
    {
        var cubeRenderer = plane.GetComponent<Renderer>();

        //Get position of the hand
        handPos3 = hand.transform.position;
        if (firstHand)
        {
            handLerpPos3 = handPos3;
            firstHand = false;
        }

        handLerpPos3 = Vector3.Lerp(handLerpPos3, handPos3, smooth);
        handLerpPos2 = new Vector2(handLerpPos3.x, handLerpPos3.y);

        //If hand gets near the screen start drawing
        if (handLerpPos3.z < distanceDrawing)
        {
            CreateBrush(handLerpPos2);
            PointToMousePos(handLerpPos2);
        }
        //Else make the drawing plane slowly appear as the hand gets closer the the screen
        else
        {
            currentLineRenderer = null;
            if (handLerpPos3.z < planeDistance + distanceDrawing && handLerpPos3.z > distanceDrawing)
            {
                plane.SetActive(true);
                float a = 1 - (handLerpPos3.z - distanceDrawing) / planeDistance;
                Color customColor = new Color(1f, 1f, 1f, a);
                cubeRenderer.material.SetColor("_BaseColor", customColor);
                drawingPause = true;
            }
            else if (handLerpPos3.z < distanceDrawing)
            {
                plane.SetActive(true);
                Color customColor = new Color(1f, 1f, 1f, 1);
                cubeRenderer.material.SetColor("_BaseColor", customColor);
            }
            else
            {
                plane.SetActive(false);
            }
        }
    }

    //Create drawing brush
    void CreateBrush(Vector2 handPos)
    {
        GameObject brushInstance = Instantiate(brush);
        currentLineRenderer = brushInstance.GetComponent<LineRenderer>();
        brushInstance.layer = LayerMask.NameToLayer("Drawing");
        currentLineRenderer.positionCount = 2;
        if (drawingPause)
        {
            drawingPause = false;
            lastPos = handPos;
        }
        currentLineRenderer.SetPosition(0, lastPos);
        currentLineRenderer.SetPosition(1, handPos);

        // Make brush bigger as hand comes closer the the screen
        float brushWidth;
        if (distanceDrawing - handLerpPos3.z > 0.3)
        {
            brushWidth = 0.03f;
        }
        else
        {
            brushWidth = (distanceDrawing - handLerpPos3.z) / 10;
        }

        currentLineRenderer.SetWidth(brushWidth, brushWidth);

        brushInstance.transform.SetParent(this.transform);
        lastPos = handPos;

    }

    //Add point to draw line
    void AddAPoint(Vector2 pointPos)
    {
        currentLineRenderer.positionCount++;
        int positionIndex = currentLineRenderer.positionCount - 1;
        currentLineRenderer.SetPosition(positionIndex, pointPos);
    }

    //Add point if position changes
    void PointToMousePos(Vector2 handPos)
    {
        if (lastPos != handPos)
        {
            AddAPoint(handPos);
            lastPos = handPos;
        }
    }

    //Clear drawing on user side for both describer and guesser brush (depending on mode of the user)
    public void ClearDrawing()
    {
        if (GameObject.Find("BrushDescriber(Clone)"))
        {
            var brushstrokes = Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name == "BrushDescriber(Clone)");
        
            foreach (var brush in brushstrokes)
            {
                Destroy(brush);
            }
        }
        else if (GameObject.Find("BrushGuesser(Clone)"))
        {
            var brushstrokes = Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name == "BrushGuesser(Clone)");

            foreach (var brush in brushstrokes)
            {
                Destroy(brush);
            }
        }
    }

    //Select right hand to draw
    public void RightHandDrawing()
    {
        ChoosenHandDrawing = "WristRight";
    }

    //Select left hand to draw
    public void LeftHandDrawing()
    {
        ChoosenHandDrawing = "WristLeft";
    }
}
