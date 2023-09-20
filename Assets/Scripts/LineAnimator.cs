// This script does the line animation
/// <Summary>
/// <href="https://youtu.be/RMM3BAick4I">Source</see>
/// </Summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineAnimator : MonoBehaviour
{
    public GameObject PDTP;
    public GameObject LineObject;
    public InstructionDrawing ID;
    [SerializeField] private float animationDuration = 5f;

    private LineRenderer lineRenderer;
    private int pointsCount;
    private Vector3[] linePoints;
    private bool startAnimation = true;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        pointsCount = lineRenderer.positionCount;
        linePoints = new Vector3[pointsCount];
        for (int i = 0; i < pointsCount; i++)
        {
            linePoints[i] = lineRenderer.GetPosition(i);
        }
    }

    private void Update()
    {
        if (PDTP.activeSelf && startAnimation == true && ID.CheckPoints() == false)
        {
            StartCoroutine(AnimateLine());
            startAnimation = false;
        }
        else if (PDTP.activeSelf && startAnimation == true && ID.CheckPoints() == true)
        {
            LineObject.SetActive(false);
        }
    }

    private IEnumerator AnimateLine()
    {
        float segmentDuration = animationDuration / pointsCount;

        for (int i = 0; i < pointsCount - 1; i++)
        {
            float startTime = Time.time;

            Vector3 startPosition = linePoints[i];
            Vector3 endPosition = linePoints[i + 1];

            Vector3 pos = startPosition;
            while (pos != endPosition)
            {
                float t = (Time.time - startTime) / animationDuration;
                pos = Vector3.Lerp(startPosition, endPosition, t);

                for (int j = i + 1; j < pointsCount; j++)
                {
                    lineRenderer.SetPosition(j, pos);
                }
                yield return null;
            }
        }
        startAnimation = true;
    }

    public void ResetLineAnimation()
    {
        LineObject.SetActive(true);
        startAnimation = true;
    }
}
