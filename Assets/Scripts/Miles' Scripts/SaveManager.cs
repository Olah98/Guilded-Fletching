using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using System.Xml;
using System.Xml.Serialization;

public class SaveManager : MonoBehaviour
{

    public static SaveManager instance;
    public SaveData activeSave;
    public bool hasLoaded;

    private GameObject[] switches;
    // Start is called before the first frame update

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
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

        
    }

    public void Save()
    {
   
        //sets location to game path
        string dataPath = Application.persistentDataPath;
        //Sets save data type
        var serializer = new XmlSerializer(typeof(SaveData));
        //Creates file and name
        var stream = new FileStream(dataPath + "/" + activeSave.saveName + ".save", FileMode.Create);
        
        //Stores data
        serializer.Serialize(stream, activeSave);
        //Ends saving process
        stream.Close();

        Debug.Log("Saved");
    }

    public void Load()
    {
        
        //Sets location to game path
        string dataPath = Application.persistentDataPath;

        //Check that file exists before loading and is of the same scene
        if (System.IO.File.Exists(dataPath + "/" + activeSave.saveName + ".save"))
        {
            //Sets save data type
            var serializer = new XmlSerializer(typeof(SaveData));
            //Sets file path
            var stream = new FileStream(dataPath + "/" + activeSave.saveName + ".save", FileMode.Open);
            //
            activeSave = serializer.Deserialize(stream) as SaveData;
            stream.Close();

            Debug.Log("Loaded");
            //Load varaibles
            LoadVariables();
            //Checks if it has been loaded
            hasLoaded = true;

        }
    }

    public void DeleteSave()
    {
        //Detects File location to game location
        string dataPath = Application.persistentDataPath;

        if (System.IO.File.Exists(dataPath + "/" + activeSave.saveName + ".save"))
        {
            //Deletes file save.
            File.Delete(dataPath + "/" + activeSave.saveName + ".save");
        }
    }

    //Saves variables onto each individual object.
    public void SaveVariables()
    {

        switches = GameObject.FindGameObjectsWithTag("Interactable");  
        instance.activeSave.switchBools = new bool[switches.Length];

        
        for (int i = 0; i < switches.Length; i++)
        {
            if (switches[i].GetComponent<Switch>() != null)
            {
                instance.activeSave.switchBools[i] = switches[i].GetComponent<Switch>().isFlipped;
            }
        }
        
       
    }

    //Loads the set variables into the scene after the laod.
    public void LoadVariables()
    {
        if (instance.activeSave.sceneName == SceneManager.GetActiveScene().name)
        switches = GameObject.FindGameObjectsWithTag("Interactable");

        for (int i = 0; i < switches.Length; i++)
        {
            if (switches[i].GetComponent<Switch>() != null)
            {
                switches[i].GetComponent<Switch>().isFlipped = instance.activeSave.switchBools[i];
            }
        }
    }
}



[System.Serializable]
public class SaveData 
{
    public string saveName;

    public string sceneName;

    //Respawn Position of Player
    public Vector3 respawnPos;

    //Determines what bools are flipped on or off at start of save
    public bool[] switchBools;

}
