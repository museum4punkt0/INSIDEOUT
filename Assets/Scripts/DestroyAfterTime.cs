/// <Summary>
/// <href="https://owlcation.com/stem/How-to-fade-out-a-GameObject-in-Unity">Source</see>
/// </Summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    public float destroyTime = 5.0f;
    public float fadeDelay = 5.0f;
    public float fadeSpeed = 4.0f;
    private float timeDelta = 0.0f;
    private bool fadeOut = false;
    private bool fade = true;

    //Destroys the brush strokes after some time
    void Update()
    {
        Object.Destroy(this.gameObject, destroyTime);
        timeDelta += Time.deltaTime;
        
        if (timeDelta > fadeDelay && fade == true)
        {
            fadeOut = true;
            fade = false;
        }
    
        // Fades out the brush strokes over time
        if (fadeOut)
        {
            Color objectColor = this.GetComponent<Renderer>().material.color;
            float fadeAmount = objectColor.a - (fadeSpeed * Time.deltaTime);

            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);

            this.GetComponent<Renderer>().material.color = objectColor;

            if(objectColor.a <= 0)
            {
                fadeOut = false;
            }
        }
    }
}
