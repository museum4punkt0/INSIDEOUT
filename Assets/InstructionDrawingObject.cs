using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class InstructionDrawingObject : MonoBehaviour
{
    public Image ObjectT;
    public ChangeModus CM;

    private int numberObjects;
    private int tutorialMuseumObject;

    void Start()
    {
        var filesOfObjects = Resources.LoadAll("Museumobjects/");
        numberObjects = filesOfObjects.Length / 2;
        tutorialMuseumObject = Random.Range(0, numberObjects);

        Sprite tutorialObjectSprite = Resources.Load<Sprite>("Museumobjects/" + tutorialMuseumObject);
        ObjectT.sprite = tutorialObjectSprite;
    }


    public void NextTutorialObject()
    {
        CM.ClearDrawing();
        tutorialMuseumObject = Random.Range(0, numberObjects);

        Sprite tutorialObjectSprite = Resources.Load<Sprite>("Museumobjects/" + tutorialMuseumObject);
        ObjectT.sprite = tutorialObjectSprite;
    }
}
