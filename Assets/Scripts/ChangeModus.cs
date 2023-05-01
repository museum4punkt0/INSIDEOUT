// This script manages the changes of the different modi the communication of the station, most of the game logic as well as displaying of the different panels and gameobjects

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using UnityEditor;
using TMPro;
using System.Linq;
using Random = UnityEngine.Random;
using com.rfilkov.kinect;

public class ChangeModus : MonoBehaviour
{
    // Screen saver, Tutorial, Game
    public GameObject InsideOut;
    public GameObject Tutorial;
    public GameObject Game;

    // Tutorial
    public GameObject StartPanel;
    public GameObject StartTutorialHand;
    public GameObject StartTutorialButton;
    public GameObject StartTutorialLeftRight;
    public GameObject DrawingTutorialPantomimeDrawing;
    public GameObject DrawingTutorialDrawing;
    public GameObject DrawingTutorialDelete;
    public GameObject DrawingTutorialPoints;
    public GameObject DrawingTutorialObject;

    // Game invite
    public GameObject StartPlayingPanel;
    public GameObject PlayTogether;
    public GameObject WaitingTogether;

    public TMP_Text PanelPlayText;
    public TMP_Text MuseumText;
    public TMP_Text ExplanationText;

    // Game
    public GameObject GuesserObjects;
    public GameObject DescriberObject;

    public GameObject Play;
    public GameObject PlayButton;
    public GameObject Next;
    public GameObject Waiting;

    public GameObject Delete;

    public GameObject PantomimeEmoji;
    public GameObject DrawingEmoji;

    public Image Object1; //Guesser Object 1
    public Image Object2; //Guesser Object 2
    public Image Object3; //Guesser Object 3
    public Image ObjectD; //Describer Object
    public Image ObjectR; //Result Object

    public JSONReader jsonObjects;

    public GameObject Right;
    public GameObject Wrong;

    public GameObject OpenMenu;
    public GameObject Menu;

    public GameObject Drawing;

    public GameObject BrushDescriber;
    public GameObject BrushGuesser;

    public GameObject Mouse;

    public SendStreamMessageSample SSMS;
    public KinectManager KM;

    private int[] Modus = { -1, 1, 0, -1, 2, 0, -1, 3, 0, -1, 4, 0 };

    int musobj;
    int rightMuseumObject = 0;
    int[] museumsObjects = { 0, 0, 0 };
    int choosenObject = 0;
    int numberObjects;
    bool objectPlaced = false;
    bool objectSent = false;
    bool forceNext = false;
    int correctButton = 0;

    string[] messages = { "o", "0" };
    string messageIncom = "o,0";
    int modeIncome;
    int sentObjectID;
    int compaireObjectID = -1;
    int countModus = 0;
    int compaireMode = 0;

    int firstGuess = -1;
    int secondGuess = -1;

    bool gameStart = false;
    public float inactiveUserTime = 6.0f;
    private float waitingUserTime = 0.0f;
    bool userDetect = false;

    void Start()
    {
        // Get museum objects
        var filesOfObjs = Resources.LoadAll("Museumobjects/");
        numberObjects = filesOfObjs.Length / 2;

        // Do default screen
        DefaultScreen();
    }

    void Update()
    {
        //Check Messages
        if (messageIncom != SendStreamMessageSample.messagestring)
        {
            messageIncom = SendStreamMessageSample.messagestring;
            messages = messageIncom.Split(",");
            Debug.Log(messages[0] + "," + Modus[countModus]);
            if (messages[0] != null)
            {
                int.TryParse(messages[0], out modeIncome);
            }
            if (messages[1] != null)
            {
                int.TryParse(messages[1], out sentObjectID);
                
            }
        }

        // Look if user is detected on our side
        if (KM.IsUserDetected(0) == true)
        {
            waitingUserTime = 0.0f;
            userDetect = true;
        }
        else
        {
            waitingUserTime = waitingUserTime + Time.deltaTime;
            if (waitingUserTime >= inactiveUserTime)
            {
                userDetect = false;
            }
        }

        /// Modi:
        ///////// -4: No User
        ///////// -3: User in Tutorial
        ///////// -2: Ready playing other side
        ///////// -1: Start Game
        /////////  0: Show Result
        /////////  1: Pantomime, Describer
        /////////  2: Pantomime, Guesser
        /////////  3: Drawing, Describer
        /////////  4: Drawing, Guesser

        //User Detected on this side
        if (userDetect == true)
        {
            //Are both users ready to play
            if (modeIncome >= -2 && gameStart == true)
            {
                ResetInvite();

                if (Modus[countModus] == -1)
                {
                    BeginScene();
                    if (objectSent == false)
                    {
                        objectSent = true;
                        SendRandomObject();
                    }
                }
                else if (Modus[countModus] == 0)
                {
                    NextModus();
                    if (objectPlaced == true)
                    {
                        objectPlaced = false;
                    }
                    objectSent = false;
                }
                else if (Modus[countModus] == 1 && modeIncome == 2)
                {
                    Modus1();
                    forceNext = true;
                }
                else if (Modus[countModus] == 2 && modeIncome == 1)
                {
                    Modus2();
                }
                else if (Modus[countModus] == 3 && modeIncome == 4)
                {
                    Modus3();
                    forceNext = true;
                }
                else if (Modus[countModus] == 4 && modeIncome == 3)
                {
                    Modus4();
                }
                if (modeIncome == 0 && forceNext == true && Modus[countModus] > 0)
                {
                    NextCount(0);
                    forceNext = false;
                }
            }
            //Set other user back to tutorial after playing together
            else if (modeIncome >= -1 && gameStart == false)
            {
                countModus = 0;
                ResetStartGame();
                ResetStartGameOtherUser();

            }
            //Send Invite to user on other side
            else if (modeIncome == -3 && gameStart == false || modeIncome == -2 && gameStart == false)
            {
                countModus = 0;
                OurUser();
                GameInvite();

                Modus[1] = 1;
                Modus[4] = 2;
                Modus[7] = 3;
                Modus[10] = 4;
                Debug.Log(Modus);

            }
            //Wait for other user to play together
            else if (modeIncome == -3 && gameStart == true)
            {
                countModus = 0;
                GameInvite();
                StartGame();

                Modus[1] = 2;
                Modus[4] = 1;
                Modus[7] = 4;
                Modus[10] = 3;
                Debug.Log(Modus);
            }
            //No user on other side
            else if (modeIncome <= -4)
            {
                countModus = 0;
                OurUser();
                ResetInvite();
                ResetStartGame();
            }
        }
        //No User detected on this side
        else
        {
            countModus = 0;
            NoUser();
            ResetStartGame();
        }
    }

    // Default screen
    public void DefaultScreen()
    {
        ClearDrawing();

        InsideOut.SetActive(true);
        Tutorial.SetActive(false);
        Game.SetActive(false);

        ResetInvite();
        ResetTutorial();
    }

    // No user on our side
    public void NoUser()
    {
        SSMS.SendMessage(-4 + "," + rightMuseumObject);
        Debug.Log(-4 + "," + rightMuseumObject);

        ClearDrawing();

        InsideOut.SetActive(true);
        Tutorial.SetActive(false);
        Game.SetActive(false);

        Menu.SetActive(false);
        OpenMenu.SetActive(false);

        ResetInvite();
        ResetTutorial();
    }

    // User on our side
    public void OurUser()
    {
        SSMS.SendMessage(-3 + "," + rightMuseumObject);
        Debug.Log(-3 + "," + rightMuseumObject);

        InsideOut.SetActive(false);
        Tutorial.SetActive(true);
        Game.SetActive(false);

        Menu.SetActive(false);
        OpenMenu.SetActive(false);

        DrawingScript drawingScript = Drawing.GetComponent<DrawingScript>();
        drawingScript.brush = BrushDescriber;
    }

    // Activate when user on both sides
    public void GameInvite()
    {
        StartPlayingPanel.SetActive(true);
    }

    // Resets the game invite
    public void ResetInvite()
    {
        PlayTogether.SetActive(true);
        WaitingTogether.SetActive(false);
        StartPlayingPanel.SetActive(false);
    }

    // Resets the tutorial
    public void ResetTutorial()
    {
        StartPanel.SetActive(true);
        StartTutorialHand.SetActive(false);
        StartTutorialButton.SetActive(false);
        StartTutorialLeftRight.SetActive(false);
        DrawingTutorialDrawing.SetActive(false);
        DrawingTutorialPantomimeDrawing.SetActive(false);
        DrawingTutorialDelete.SetActive(false);
        DrawingTutorialPoints.SetActive(false);
        DrawingTutorialObject.SetActive(false);
    }

    // Our player is ready to start the game
    public void StartGame()
    {
        SSMS.SendMessage(-2 + "," + rightMuseumObject);
        Debug.Log(-2 + "," + rightMuseumObject);

        PlayTogether.SetActive(false);
        WaitingTogether.SetActive(true);

        Menu.SetActive(false);
        OpenMenu.SetActive(false);

        PlayButton.SetActive(true);

        gameStart = true;
    }

    // Reset that the user wants to play together
    public void ResetStartGame()
    {
        gameStart = false;
    }

    // Resets that the other user wants to play the game
    public void ResetStartGameOtherUser()
    {
        SSMS.SendMessage(-4 + "," + rightMuseumObject);
        Debug.Log(-4 + "," + rightMuseumObject);

        Menu.SetActive(false);
        OpenMenu.SetActive(false);
    }

    //Game: Starting Scene
    public void BeginScene()
    {
        ClearDrawing();

        InsideOut.SetActive(false);
        Tutorial.SetActive(false);
        Game.SetActive(true);

        GuesserObjects.SetActive(false);
        DescriberObject.SetActive(false);
        OpenMenu.SetActive(true);

        Play.SetActive(true);
        Next.SetActive(false);
        Waiting.SetActive(false);

        if(countModus < Modus.Length-1)
        {
            if (Modus[countModus + 1] == 1)
            {
                PanelPlayText.text = "Beschreibe das folgende Objekt durch Pantomime.";
            }
            else if (Modus[countModus + 1] == 3)
            {
                PanelPlayText.text = "Beschreibe das folgende Objekt durch Malen.";
            }
            else if (Modus[countModus + 1] == 2 || Modus[countModus + 1] == 4)
            {
                PanelPlayText.text = "Errate das folgende Objekt.";
            }
        }

        Delete.SetActive(false);

        PantomimeEmoji.SetActive(false);
        DrawingEmoji.SetActive(false);

        Right.SetActive(false);
        Wrong.SetActive(false);

        Drawing.SetActive(false);

    }

    //Game: Next Modus
    public void NextModus()
    {
        ClearDrawing();

        GuesserObjects.SetActive(false);
        DescriberObject.SetActive(false);

        Play.SetActive(false);
        PlayButton.SetActive(true);
        Next.SetActive(true);
        Waiting.SetActive(false);

        Delete.SetActive(false);
        Menu.SetActive(false);
        OpenMenu.SetActive(false);

        PantomimeEmoji.SetActive(false);
        DrawingEmoji.SetActive(false);

        Drawing.SetActive(false);
    }


    //Game: Pantomime, Describer
    public void Modus1()
    {
        GuesserObjects.SetActive(false);
        DescriberObject.SetActive(true);

        Play.SetActive(false);
        Next.SetActive(false);
        Waiting.SetActive(false);

        Delete.SetActive(false);
        Menu.SetActive(false);
        OpenMenu.SetActive(false);

        PantomimeEmoji.SetActive(true);
        DrawingEmoji.SetActive(false);

        Right.SetActive(false);
        Wrong.SetActive(false);

        Drawing.SetActive(false);

        if (objectPlaced == false)
        {
            objectPlaced = true;
        }
        objectSent = false;
    }

    //Game: Pantomime, Guesser
    public void Modus2()
    {
        GuesserObjects.SetActive(true);
        DescriberObject.SetActive(false);

        Play.SetActive(false);
        Next.SetActive(false);
        Waiting.SetActive(false);

        Delete.SetActive(false);
        Menu.SetActive(false);
        OpenMenu.SetActive(false);

        PantomimeEmoji.SetActive(true);
        DrawingEmoji.SetActive(false);

        Right.SetActive(false);
        Wrong.SetActive(false);

        Drawing.SetActive(false);

        if (objectPlaced == false)
        {
            objectPlaced = true;
            PlaceThreeObjects();
        }
        objectSent = false;
    }


    //Game: Drawing, Describer
    public void Modus3()
    {
        GuesserObjects.SetActive(false);
        DescriberObject.SetActive(true);

        Play.SetActive(false);
        Next.SetActive(false);
        Waiting.SetActive(false);

        Delete.SetActive(true);
        Menu.SetActive(false);
        OpenMenu.SetActive(false);

        PantomimeEmoji.SetActive(false);
        DrawingEmoji.SetActive(true);

        Right.SetActive(false);
        Wrong.SetActive(false);

        Drawing.SetActive(true);

        if (objectPlaced == false)
        {
            objectPlaced = true;
        }
        objectSent = false;

        DrawingScript drawingScript = Drawing.GetComponent<DrawingScript>();
        drawingScript.brush = BrushDescriber;
    }

    //Game: Drawing, Guesser
    public void Modus4()
    {
        GuesserObjects.SetActive(true);
        DescriberObject.SetActive(false);

        Play.SetActive(false);
        Next.SetActive(false);
        Waiting.SetActive(false);

        Delete.SetActive(true);
        Menu.SetActive(false);
        OpenMenu.SetActive(false);

        PantomimeEmoji.SetActive(false);
        DrawingEmoji.SetActive(true);

        Right.SetActive(false);
        Wrong.SetActive(false);

        Drawing.SetActive(true);

        if (objectPlaced == false)
        {
            objectPlaced = true;
            PlaceThreeObjects();
        }
        objectSent = false;

        DrawingScript drawingScript = Drawing.GetComponent<DrawingScript>();
        drawingScript.brush = BrushGuesser;


    }

    //Game: Count to next mode in game
    public void NextCount(int next)
    {
        //Next count or restart count
        if (countModus < Modus.Length-1)
        {
            Debug.Log(Modus.Length);
            countModus++;
            SSMS.SendMessage(Modus[countModus]+","+rightMuseumObject);
            Debug.Log(countModus+","+Modus[countModus] + "," + rightMuseumObject);
        }
        else
        {
            countModus = 0;
            SSMS.SendMessage(Modus[countModus] + "," + rightMuseumObject);
        }

        // Guesser Result
        if (next != 0)
        {
            SSMS.SendMessage(Modus[countModus] + "," + museumsObjects[next - 1]);
            if (sentObjectID == museumsObjects[next - 1])
            {
                Debug.Log("Object: " + sentObjectID + ", " + museumsObjects[next - 1]);
                if (Modus[countModus] == 0)
                {
                    Right.SetActive(true);
                }
                    
            }
            else
            {
                Debug.Log("Object: " + sentObjectID + ", " + museumsObjects[next - 1]);
                if (Modus[countModus] == 0)
                {
                    Wrong.SetActive(true);
                }
                    
            }

            Sprite objectSpriteR = Resources.Load<Sprite>("Museumobjects/" + sentObjectID);
            ObjectR.sprite = objectSpriteR;
            ExplanationText.text = jsonObjects.myObjectList.museumobject[sentObjectID].description;
            MuseumText.text = jsonObjects.myObjectList.museumobject[sentObjectID].museum;

        }
        // Describer Result
        else
        {
            SSMS.SendMessage(Modus[countModus] + "," + rightMuseumObject);
            if (rightMuseumObject == sentObjectID)
            {
                Debug.Log("Object: " + rightMuseumObject + ", " + sentObjectID);
                if (Modus[countModus] == 0)
                {
                    Right.SetActive(true);
                }
                    
            }
            else
            {
                Debug.Log("Object: " + rightMuseumObject + ", " + sentObjectID);
                if (Modus[countModus] == 0)
                {
                    Wrong.SetActive(true);
                }
                    
            }

            Sprite objectSpriteR = Resources.Load<Sprite>("Museumobjects/" + rightMuseumObject);
            ObjectR.sprite = objectSpriteR;
            ExplanationText.text = jsonObjects.myObjectList.museumobject[rightMuseumObject].description;
            MuseumText.text = jsonObjects.myObjectList.museumobject[rightMuseumObject].museum;
        }  
    }

    //Game: Describer chooses and sends random object to other side
    public void SendRandomObject()
    {
        rightMuseumObject = Random.Range(0, numberObjects);
        SSMS.SendMessage(Modus[countModus] + "," + rightMuseumObject);
        PlaceOneObject();
    }

    //Game: Random object is placed on Describers side
    public void PlaceOneObject()
    {
        Sprite objectSprite = Resources.Load<Sprite>("Museumobjects/" + rightMuseumObject);
        ObjectD.sprite = objectSprite;
    }

    //Game: Place three random objects on Guessers side
    public void PlaceThreeObjects()
    {
        correctButton = Random.Range(0, 3);

        firstGuess = Random.Range(0, numberObjects);
        secondGuess = Random.Range(0, numberObjects);

        while (firstGuess == sentObjectID)
        {
            firstGuess = Random.Range(0, numberObjects);
        }
        while (secondGuess == sentObjectID || secondGuess == firstGuess)
        {
            secondGuess = Random.Range(0, numberObjects);
        }

        
        if (correctButton == 0)
        {
            Sprite objectSprite1 = Resources.Load<Sprite>("Museumobjects/" + sentObjectID);
            Object1.sprite = objectSprite1;
            museumsObjects[0] = sentObjectID;

            Sprite objectSprite2 = Resources.Load<Sprite>("Museumobjects/" + firstGuess);
            Object2.sprite = objectSprite2;
            museumsObjects[1] = firstGuess;

            Sprite objectSprite3 = Resources.Load<Sprite>("Museumobjects/" + secondGuess);
            Object3.sprite = objectSprite3;
            museumsObjects[2] = secondGuess;

            Debug.Log("Richtig:" + sentObjectID + ", " + firstGuess + ", " + secondGuess );
        }
        else if (correctButton == 1)
        {
            Sprite objectSprite1 = Resources.Load<Sprite>("Museumobjects/" + firstGuess);
            Object1.sprite = objectSprite1;
            museumsObjects[0] = firstGuess;

            Sprite objectSprite2 = Resources.Load<Sprite>("Museumobjects/" + sentObjectID);
            Object2.sprite = objectSprite2;
            museumsObjects[1] = sentObjectID;

            Sprite objectSprite3 = Resources.Load<Sprite>("Museumobjects/" + secondGuess);
            Object3.sprite = objectSprite3;
            museumsObjects[2] = secondGuess;

            Debug.Log(firstGuess +", Richtig:" + sentObjectID + ", " + secondGuess);
        }
        else if (correctButton == 2)
        {
            Sprite objectSprite1 = Resources.Load<Sprite>("Museumobjects/" + secondGuess);
            Object1.sprite = objectSprite1;
            museumsObjects[0] = secondGuess;

            Sprite objectSprite2 = Resources.Load<Sprite>("Museumobjects/" + firstGuess);
            Object2.sprite = objectSprite2;
            museumsObjects[1] = firstGuess;

            Sprite objectSprite3 = Resources.Load<Sprite>("Museumobjects/" + sentObjectID);
            Object3.sprite = objectSprite3;
            museumsObjects[2] = sentObjectID;

            Debug.Log( secondGuess + ", " + firstGuess  + ", Richtig:" + sentObjectID);
        }

    }

    // Delete drawings
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
}
