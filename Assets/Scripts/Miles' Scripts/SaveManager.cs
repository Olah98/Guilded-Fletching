using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using System.Xml;
using System.Xml.Serialization;

//Author: Miles Gomez
//Changes made 3/4/2021

public class SaveManager : MonoBehaviour
{

    public static SaveManager instance;
    public SaveData activeSave;
    public bool hasLoaded;

    private GameObject[] switches;
    private GameObject[] enemies;
    private GameObject player;
    // Start is called before the first frame update

    private void Awake()
    {
        
        if (instance==null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance!=this)
        {
            Destroy(gameObject);
        }
       

        Load();
    }
    
    void Start()
    {
      
    }
    private void OnLevelWasLoaded(int level)
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Debug.Log("its a me the scene");
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            DeleteSave();
        }
    }

    public string FileName()
    {
        //sets datapath to save user folder
        string dataPath = Application.persistentDataPath;
        //file name
        return (dataPath + "/" + activeSave.saveName + ".save");
    }

    public void Save()
    {  
        //Sets save data type
        var serializer = new XmlSerializer(typeof(SaveData));
        //Creates file and name
        var stream = new FileStream(FileName(), FileMode.Create);
        
        //Stores data
        serializer.Serialize(stream, activeSave);
        //Ends saving process
        stream.Close();

    
    }

    public void Load()
    {
        
        //Sets location to game path

        //Check that file exists before loading and is of the same scene
        if (File.Exists(FileName()))
        {
            //Sets save data type
            var serializer = new XmlSerializer(typeof(SaveData));
            //Sets file path
            var stream = new FileStream(FileName(), FileMode.Open);
            //
            activeSave = serializer.Deserialize(stream) as SaveData;
            stream.Close();

            
            //Load varaibles
            LoadVariables();
            //Checks if it has been loaded
            hasLoaded = true;

        }
        instance.activeSave.unsavedDead.Clear();
    }

    public void DeleteSave()
    {
        //Detects File location to game location
       

        if (File.Exists(FileName()))
        {
            //Deletes file save.
            
            File.Delete(FileName());
            ResetVariables();
          
        }

        //delete variables
    }

    //Saves variables onto each individual object.
    public void SaveVariables()
    {
        //Saves switch bools
        switches = GameObject.FindGameObjectsWithTag("Interactable");  
        instance.activeSave.switchBools = new bool[switches.Length];

        for (int i = 0; i < switches.Length; i++)
        {
            if (switches[i].GetComponent<Switch>() != null)
            {
                instance.activeSave.switchBools[i] = switches[i].GetComponent<Switch>().isFlipped;
            }
        }

        //Saves enemy bools for isDead
        if (instance.activeSave.unsavedDead.Count != 0)
        {
            foreach (string enemy in instance.activeSave.unsavedDead)
            {
                instance.activeSave.enemyDead.Add(enemy);
            }

            instance.activeSave.unsavedDead.Clear();
        }

        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            instance.activeSave.equippedType = player.GetComponent<Quiver>().GetArrowType(); //By Warren
            instance.activeSave.recordStandard = player.GetComponent<Quiver>().GetArrowTypeShot(0);
            instance.activeSave.recordBramble = player.GetComponent<Quiver>().GetArrowTypeShot(1);
            instance.activeSave.recordWarp = player.GetComponent<Quiver>().GetArrowTypeShot(2);
            instance.activeSave.recordAirburst = player.GetComponent<Quiver>().GetArrowTypeShot(3);
            //instance.activeSave.loadoutSaved = player.GetComponent<Quiver>().ReportLoadout();
        }
    }

    //Loads the set variables into the scene after the laod.
    public void LoadVariables()
    {
        if (instance.activeSave.sceneName == SceneManager.GetActiveScene().name)
        {

            //Switches
            switches = GameObject.FindGameObjectsWithTag("Interactable");

            for (int i = 0; i < switches.Length; i++)
            {
                if (switches[i].GetComponent<Switch>() != null)
                {
                    switches[i].GetComponent<Switch>().isFlipped = instance.activeSave.switchBools[i];
                }
            }

            //Active Checkpoint
            if (GameObject.Find(activeSave.activeCheckpoint)!=null)
            {
                GameObject checkPoint;
                checkPoint = GameObject.Find(activeSave.activeCheckpoint);
                checkPoint.GetComponent<Checkpoint>().activeCheckpoint = true;
            }

            //Enemies
            //clear unsaved enemies list
            instance.activeSave.unsavedDead.Clear();
            //kill targets in deadenemy list
            foreach(string enemy in instance.activeSave.enemyDead)
            {
                if (GameObject.Find(enemy)!=null)
                {
                    GameObject killTarget;
                    killTarget = GameObject.Find(enemy);
                    Destroy(killTarget);
                }
            }

            // set options
            //player = GameObject.FindGameObjectWithTag("Player");
            //if (player != null)
            //{
            //    player.GetComponent<Quiver>().EquipType(instance.activeSave.equippedType); //By Warren
            //    instance.activeSave.equippedType = 0;
            //}
        }
    }

    public void ResetVariables()
    {
        instance.activeSave.sceneName = default;
        instance.activeSave.activeCheckpoint = default;

        instance.activeSave.respawnPos = default;

        instance.activeSave.switchBools = new bool[0];

        instance.activeSave.unsavedDead.Clear();
        instance.activeSave.enemyDead.Clear();
    }

    /// <summary>
    /// (Added by Christian): public static helper that can use FindObjectsOfType<> 
    /// from a script that doesn't implement the MonoBehavior interface (SavedData).
    /// </summary>
    /// <returns>List of all objects in scene that contains an AudioSource</returns>
    public static AudioSource[] GetAllAudioInScene() {
        return FindObjectsOfType<AudioSource>();
    }
}



[System.Serializable]
public class SaveData 
{
    public string saveName;

    public string sceneName;

    public string activeCheckpoint;

    //Respawn Position of Player
    public Vector3 respawnPos;

    //Determines what bools are flipped on or off at start of save
    public bool[] switchBools;

    public List<string> unsavedDead;
    public List<string> enemyDead;

    public int equippedType;
    
    public int recordStandard;
    public int recordBramble;
    public int recordWarp;
    public int recordAirburst;
    //public int[,] loadoutSaved = new int[4, 2];
}