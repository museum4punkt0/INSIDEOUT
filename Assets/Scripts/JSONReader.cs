//Reads data out of json file
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSONReader : MonoBehaviour
{
    public TextAsset textJSON;

    [System.Serializable]
    public class Museumobject
    {
        public int id;
        public string description;
        public string museum;
    }

    [System.Serializable]
    public class ObjectList
    {
        public Museumobject[] museumobject;
    }
    public ObjectList myObjectList = new ObjectList();
    
    void Start()
    {
        myObjectList = JsonUtility.FromJson<ObjectList>(textJSON.text);
    }
}
