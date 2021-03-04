using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

public class SaveManager : MonoBehaviour
{

    public static SaveManager instance;
    public SaveData activeSave;
    public bool hasLoaded;

    public bool[] switchBools;
    // Start is called before the first frame update

    private void Awake()
    {
        instance = this;

        Load();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Save();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Load();
        }
    }

    public void Save()
    {

        string dataPath = Application.persistentDataPath;

        var serializer = new XmlSerializer(typeof(SaveData));
        var stream = new FileStream(dataPath + "/" + activeSave.saveName + ".save", FileMode.Create);
        //Name File
        serializer.Serialize(stream, activeSave);
        stream.Close();

        Debug.Log("Saved");
    }

    public void Load()
    {
        string dataPath = Application.persistentDataPath;

        if (System.IO.File.Exists(dataPath + "/" + activeSave.saveName + ".save"))
        {
            var serializer = new XmlSerializer(typeof(SaveData));
            var stream = new FileStream(dataPath + "/" + activeSave.saveName + ".save", FileMode.Open);

            activeSave = serializer.Deserialize(stream) as SaveData;
            stream.Close();

            Debug.Log("Loaded");

            hasLoaded = true;
        }
    }

    public void DeleteSave()
    {
        string dataPath = Application.persistentDataPath;

        if (System.IO.File.Exists(dataPath + "/" + activeSave.saveName + ".save"))
        {
            File.Delete(dataPath + "/" + activeSave.saveName + ".save");
        }
    }

    //Saves variables onto each individual object.
    public void SaveVariables()
    {
        
        instance.GetComponent<SaveData>().switches = GameObject.FindGameObjectsWithTag("Interactable");
        switchBools = new bool[instance.GetComponent<SaveData>().switches.Length];
        for (int i = 0; i < instance.GetComponent<SaveData>().switches.Length; i++)
        {
            if (instance.GetComponent<SaveData>().switches[i].GetComponent<Switch>() != null)
            {
                switchBools[i] = instance.GetComponent<SaveData>().switches[i].GetComponent<Switch>().isFlipped;
            }
        }
        
       
    }
}



[System.Serializable]
public class SaveData
{
    public string saveName;

    public Vector3 respawnPos;

    public GameObject[] switches;

    public GameObject[] switchBools;

}
