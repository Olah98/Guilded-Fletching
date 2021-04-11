/*
Author: Christian Mullins
Date: 03/01/2021
Summary: Scriptable object type save file that can store, retrieve, and edit 
    itself. Since BinaryFormatter doesn't like ScriptableObjects, DataWrapper will 
*/
using System;
using System.IO;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class SavedData {
    /*
        Variables marked "s_" are serializable variables and must be reworked 
        to be used as their respective MonoBehaviors class.
    */
    public string saveName = String.Empty;
    [Header("Player Values")]
    public int playerHealth = 100;
    public SerializableQuiver s_Quiver = new SerializableQuiver();
    [Header("Level Values")]
    //public int maxLevelReached = 1;           // use in later implementation
    public int currentLevel = 1;
    public SerializableVector3 s_Vector3;
    [Header("Option Values")]
    [Range(0.1f, 1.0f)] public float graphicsQuality = 1f;
    [Range(0.0f, 1.0f)] public float masterVol = 1f;
    [Range(0.0f, 1.0f)] public float musicVol = 1f;
    [Range(0.0f, 1.0f)] public float soundFXVol = 1f;
    [Range(0.1f, 1.0f)] public float mouseSensitivity = 0.3f;
    // 20 units left for room to zoom in
    [Range(20f, 200f)]  public float baseFOV = 60f; // default Unity FOV
    [Header("Misc Values")]
    public TimeSpan timePlayed = TimeSpan.Zero;

    public bool isNewInstance { get {
        return (timePlayed == TimeSpan.Zero && saveName == String.Empty);
    } }

    public static int currentSaveSlot = 1;
    
    private static DateTime? _startPlayTime;      // nullable
    private const string _saveStr = "SAVE_SLOT_"; // concatenate 1, 2, or 3

    #region dataSerializationAndRetrieval
    /// <summary>
    /// Returns data stored at a given slot.
    /// (w/ error handling during production)
    /// </summary>
    /// <param name="slot">1, 2, or 3 (else error thrown).</param>
    /// <returns>Data stored at the argument slot.</returns>
    public static SavedData GetDataStoredAt(in int slot) {
        
        if (slot == -1) return new SavedData();
       
        if (slot < 1 || slot > 3) throw new IndexOutOfRangeException();

        string filePath = Application.persistentDataPath + _saveStr + slot.ToString() + ".dat";
        FileStream fStream = null;
        // any file errors are due to features being implemented during 
        //  production, this turns the error into a yield, these preprocessor 
        //  directives won't build w/ the catch statement
       
        try {
       
            if (File.Exists(filePath)) {
                fStream = File.OpenRead(filePath);
                var bf = new BinaryFormatter();
                var data = (SavedData)bf.Deserialize(fStream);
                fStream.Close();
                var options = GetStoredOptionsAt(slot);
                data.graphicsQuality  = options.graphicsQuality;
                data.mouseSensitivity = options.mouseSensitivity;
                data.baseFOV          = options.baseFOV;
                data.masterVol        = options.masterVol;
                data.soundFXVol       = options.soundFXVol;
                data.musicVol         = options.musicVol;
                return data;
            }
                    
        }
        catch (Exception e) {
            // errors should only occur if a save file didn't get saved
            //  sucessfully or a variable was added to SavedData class
            Debug.LogWarning("SavedData invalid: writing new "
                + "instance...\n" + e);
            if (fStream != null) fStream.Close();
            DeleteDataSlot(slot);
        }
       
        return new SavedData();
    }

    /// <summary>
    /// Saves argument data into the argument slot.
    /// </summary>
    /// <param name="data">SavedData object to be stored.</param>
    /// <param name="slot">1, 2, or 3 (else error thrown).</param>
    public static void StoreDataAtSlot(in SavedData data, in int slot) {
        if (slot < 1 || slot > 3) throw new IndexOutOfRangeException();

        // store options to player prefs and serialize other data
        StoreOptionsAt(GetStoredOptionsAt(slot), slot);
        var fStream = File.Create(Application.persistentDataPath + _saveStr 
                                    + slot.ToString() + ".dat");
        new BinaryFormatter().Serialize(fStream, data);
        fStream.Close();
    }

    /// <summary>
    /// Saves argument data into the argument slot as an async method.
    /// </summary>
    /// <param name="data">SavedData object to be stored.</param>
    /// <param name="slot">1, 2, or 3 (else error thrown).</param>
    public static IEnumerator StoreDataAsyncAtSlot(SavedData data, int slot) {
        if (slot < 1 || slot > 3) throw new IndexOutOfRangeException();

        // store options to player prefs and serialize other data
        StoreOptionsAt(GetStoredOptionsAt(slot), slot);
        var fStream = File.Create(Application.persistentDataPath 
                                    + _saveStr + slot.ToString() + ".dat");
        var serialization = Task.Run(() => new BinaryFormatter()
                                .Serialize(fStream, data));
        while (!serialization.IsCompleted || !serialization.IsFaulted) {
            Debug.Log("Storing Data...");
            yield return new WaitForEndOfFrame();
        }
        fStream.Close();
        yield return null;
    }

    /// <summary>
    /// Revert a save slot back to empty constructor.
    /// </summary>
    /// <param name="slot">1, 2, or 3 (else error thrown).</param>
    public static void DeleteDataSlot(in int slot) {
        if (slot < 1 || slot > 3) throw new IndexOutOfRangeException();

        string filePath = Application.persistentDataPath + _saveStr + slot.ToString() + ".dat";
        if (File.Exists(filePath)) File.Delete(filePath);
        var fStream = File.OpenWrite(filePath);
        new BinaryFormatter().Serialize(fStream, new SavedData());
        fStream.Close();

        //clear option prefs from this slot
        string dataSuffix = "_" + slot.ToString();
        PlayerPrefs.DeleteKey("GraphicsQuality"  + dataSuffix);
        PlayerPrefs.DeleteKey("MouseSensitivity" + dataSuffix);
        PlayerPrefs.DeleteKey("BaseFOV"          + dataSuffix);
        PlayerPrefs.DeleteKey("MasterVolume"     + dataSuffix);
        PlayerPrefs.DeleteKey("SoundFXVolume"    + dataSuffix);
        PlayerPrefs.DeleteKey("MusicVolume"      + dataSuffix);
        PlayerPrefs.Save();
    }
    #endregion

    #region timer
    /// <summary>
    /// Start timer for this current playthrough, make sure this is called on
    /// loading a save slot.
    /// </summary>
    public static void StartTimer() {
        if (_startPlayTime == null) _startPlayTime = DateTime.Now;
        else Debug.Log("StartTimer: no action taken, timer already running");
    }

    /// <summary>
    /// Upon quitting, cut off the current play time and update timePlayed.
    /// </summary>
    public static IEnumerator CutCurrentPlayTime(SavedData data) {
        // logic check for the timer
        if (_startPlayTime == null)
            Debug.Log("SavedData: play timer has not yet been set.");
        else {
            data.timePlayed += DateTime.Now - (DateTime)_startPlayTime;
            _startPlayTime = null;
        }
        // store data
        var task = Task.Run(() => StoreDataAtSlot(data, currentSaveSlot));
        while (!task.IsCompleted) {
            Debug.Log("Saving...");
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }

    /// <summary>
    /// Retrieve timePlayed with _startPlayTime taken into account.
    /// </summary>
    /// <returns>Current total time playing.</returns>
    public TimeSpan GetCurrentPlayTime() {
        if (_startPlayTime == null) 
            return timePlayed;
        else
            return timePlayed + ((DateTime)_startPlayTime - DateTime.Now);
    }
    #endregion
    
    #region options
    /// <summary>
    /// Update current scene with option values in this current save data's
    /// </summary>
    public void SetGameVolume() {
        AudioListener.volume = masterVol;
        var sources = SaveManager.GetAllAudioInScene();
        foreach (var s in sources) {
            if (s.tag == "Player")
                s.volume = musicVol;
            else
                s.volume = soundFXVol;
        }
    }

    /// <summary>
    /// Static method of taking OptionsData type and applying them to 
    /// the current scene.
    /// </summary>
    /// <param name="options"></param>
    public static void SetOptionsInScene(in OptionsData options) {
        // set audio
        AudioListener.volume = options.masterVol;
        var sources = SaveManager.GetAllAudioInScene();
        foreach (var s in sources) {
            s.volume = (s.tag != "Player") ? options.soundFXVol 
                                           : options.musicVol;
        }
    }

    /// <summary>
    /// Static method of taking an OptionsData type to store in our currently in use
    /// data.
    /// </summary>
    /// <param name="options"></param>
    public static void StoreOptionsAt(OptionsData options, in int slot) {
        string dataSuffix = "_" + slot.ToString();
        PlayerPrefs.SetFloat("GraphicsQuality"  + dataSuffix, options.graphicsQuality);
        PlayerPrefs.SetFloat("MasterVolume"     + dataSuffix, options.masterVol);
        PlayerPrefs.SetFloat("MusicVolume"      + dataSuffix, options.musicVol);
        PlayerPrefs.SetFloat("SoundFXVolume"    + dataSuffix, options.soundFXVol);
        PlayerPrefs.SetFloat("MouseSensitivity" + dataSuffix, options.mouseSensitivity);
        PlayerPrefs.SetFloat("BaseFOV"          + dataSuffix, options.baseFOV);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Get options stored at a given save slot index.
    /// </summary>
    /// <param name="saveSlot">Index of save slot.</param>
    /// <returns>Options at a save slot packages as OptionsData class.</returns>
    public static OptionsData GetStoredOptionsAt(in int saveSlot) {
        string dataSuffix = "_" + saveSlot.ToString();
        var options = new OptionsData();
        options.graphicsQuality  = PlayerPrefs.GetFloat("GraphicsQuality"  + dataSuffix, 1.0f);
        options.mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity" + dataSuffix, 1.0f);
        options.baseFOV          = PlayerPrefs.GetFloat("BaseFOV"          + dataSuffix, 60f);
        options.masterVol        = PlayerPrefs.GetFloat("MasterVolume"     + dataSuffix, 1.0f);
        options.soundFXVol       = PlayerPrefs.GetFloat("SoundFXVolume"    + dataSuffix, 1.0f);
        options.musicVol         = PlayerPrefs.GetFloat("MusicVolume"      + dataSuffix, 1.0f);
        return options;
    }    
    #endregion
}

public class OptionsData {
    public float graphicsQuality;
    public float masterVol;
    public float musicVol;
    public float soundFXVol;
    public float mouseSensitivity;
    public float baseFOV;

    public OptionsData () {}

    public OptionsData(SavedData data) {
        this.graphicsQuality  = data.graphicsQuality;
        this.mouseSensitivity = data.mouseSensitivity;
        this.baseFOV          = data.baseFOV;
        this.masterVol        = data.masterVol;
        this.soundFXVol       = data.soundFXVol;
        this.musicVol         = data.musicVol;
    }

    // for debugging only
    [System.Obsolete]
    public void Debug_Print() {
        Debug.Log("START OPTION DEBUG");
        Debug.Log("graphicsQuality: " + graphicsQuality);
        Debug.Log("masterVol: " + masterVol);
        Debug.Log("musicVol: " + musicVol);
        Debug.Log("soundFXVol: " + soundFXVol);
        Debug.Log("mouseSensitivity: " + mouseSensitivity);
        Debug.Log("baseFOV: " + baseFOV);
        Debug.Log("END OPTION DEBUG");

    }
}
#region serializableVariables
/// <summary>
/// Used for serializing a Vector3 because MonoBehaviors can't be serialized.
/// </summary>
[Serializable]
public struct SerializableVector3 {
    public float x, y, z;

    /// <summary>
    /// Intilize a SerializableVector3 on construction.
    /// </summary>
    /// <param name="inputX">x axis</param>
    /// <param name="inputY">y axis</param>
    /// <param name="inputZ">z axis</param>
    public SerializableVector3(float inputX, float inputY, float inputZ) {
        x = inputX;
        y = inputY;
        z = inputZ;
    }

    /// <summary>
    /// Intialize by taking a Vector3 on construction.
    /// </summary>
    /// <param name="serialize">Vector3 to be serialized</param>
    public SerializableVector3(in Vector3 serialize) {
        x = serialize.x;
        y = serialize.y;
        z = serialize.z;
    }

    /// <summary>
    /// Vector3 delegate.
    /// </summary>
    /// <returns>UnityEngine based Vector3.</returns>
    public Vector3 toVector3 { get { return new Vector3(x, y, z); } }
}

/// <summary>
/// Used for serializing a Quiver because MonoBehaviors can't be serialized.
/// </summary>
[Serializable]
public class SerializableQuiver {
    public int[,] loadout = new int[4, 2];
    public int equipped;
    /*
        TODO
            In the future implement constructors based on what arrows should
            be available to the player.
    */
    public SerializableQuiver() {
        // 1s mean that this type of arrow is usable
        // second index is how many shot
        loadout = new int[,] { { 1, 0 }, { 1, 0 }, { 1, 0 }, { 1, 0 } };
        equipped = 0;
    }

    public SerializableQuiver(in int usableArrows) {
        loadout = new int[,] { { 1, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 } };
        equipped = 0;
        for (int i = 0; i < usableArrows; ++i) {
            loadout[i, 0] = 1;
        }
    }

    /// <summary>
    /// Construction based on an Quiver parameter
    /// </summary>
    /// <param name="serialize">Quiver to copy values from.</param>
    public SerializableQuiver(in Quiver serialize) {
        loadout  = serialize.getLoadout;
        equipped = serialize.getEquipped;
    }
}
#endregion
