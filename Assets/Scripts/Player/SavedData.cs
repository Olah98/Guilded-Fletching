/*
Author: Christian Mullins
Date: 03/01/2021
Summary: Scriptable object type save file that can store, retrieve, and edit 
    itself. Since BinaryFormatter doesn't like ScriptableObjects, DataWrapper will 
*/
using System;
using System.IO;
using System.Collections;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(
    fileName = "SavedData", 
    menuName = "Scriptable Objects/SavedData", 
    order = 0)]
public class SavedData : ScriptableObject {
    /* Wrapper for SavedData because BinaryFormatter doesn't like ScriptableObjects */
    [Serializable]
    private class DataWrapper : ISerializable {
        public SavedData data;

        /// <summary>
        /// Empty constructor necessary for compiling
        /// </summary>
        public DataWrapper() {}

        public DataWrapper(SavedData packing) {
            data = packing;
            var context = new StreamingContext(StreamingContextStates.File);
            var info = new SerializationInfo(typeof(DataWrapper),
                                             new FormatterConverter());
            
            this.GetObjectData(info, context);
        }
        /// <summary>
        /// The special constructor is used to deserialize values. In this case,
        ///  it recreates the original ScriptableObject.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public DataWrapper(SerializationInfo info, StreamingContext context) {
            data = (SavedData)ScriptableObject.CreateInstance(typeof(SavedData));
            if (data == null) return;
            var fields = data.GetType().GetFields(BindingFlags.Public           | BindingFlags.NonPublic 
                                                | BindingFlags.FlattenHierarchy | BindingFlags.Instance);
            foreach (var f in fields) {
                f.SetValue(data, info.GetValue(f.Name, f.FieldType));
            }
        }

        /// <summary>
        /// Inherritted by ISerializable, with code ripped off some guy on UnityForms.
        /// </summary>
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("ScriptableType", data.GetType().AssemblyQualifiedName, typeof(string));
            var fields = data.GetType().GetFields(BindingFlags.Public           | BindingFlags.NonPublic 
                                                | BindingFlags.FlattenHierarchy | BindingFlags.Instance);
            foreach(var f in fields) {
                info.AddValue(f.Name, f.GetValue(data), f.FieldType);
            }
        }
    }
    /*
        Variables marked "s_" are serializable structs and must be reworked to
        be used as MonoBehaviors.
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
    [Range(0.0f, 1.0f)]
    public float masterVol = 1f;
    [Range(0.0f, 1.0f)]
    public float musicVol = 1f;
    [Range(0.0f, 1.0f)]
    public float soundFXVol = 1f;
    [Range(0.1f, 1f)]
    public float mouseSensitivity = 0.3f;
    public float baseFOV = 60f;                  // default Unity FOV
    [Header("Misc Values")]
    public TimeSpan timePlayed = TimeSpan.Zero;

    public static int currentSaveSlot = -1;
    public static string getDataPath { get { return dataPath; } }
    
    //private DataWrapper myWrapper = new DataWrapper();
    private static DateTime? startPlayTime;      // nullable
    private const string saveStr = "SAVE_SLOT_"; // concatenate 1, 2, or 3
    private static string dataPath = Application.persistentDataPath;

    /* STATIC STORAGE AND RETRIEVAL METHODS */
    /// <summary>
    /// Returns data stored at a given slot.
    /// </summary>
    /// <param name="slot">1, 2, or 3 (else error thrown).</param>
    /// <returns>Data stored at the argument slot.</returns>
    public static SavedData GetDataStoredAt(in int slot) {
        if (slot < 1 || slot > 3) throw new IndexOutOfRangeException();
        string filePath = getDataPath + saveStr 
                          + slot.ToString() + ".dat";
        FileStream fStream = null;
        try {
            if (File.Exists(filePath)) {
                fStream = File.OpenRead(filePath);
                var bf = new BinaryFormatter();
                var wrapper = (DataWrapper)bf.Deserialize(fStream);
                fStream.Close();
                return wrapper.data;
            }
        }
        catch (SerializationException sE) {
            Debug.LogWarning("Trying to read form corrupted file" 
                         + ", overwriting to new file\n" + sE);
            if (fStream != null) fStream.Close();
            DeleteDataSlot(slot);
            //throw;
        }
        return (SavedData)ScriptableObject.CreateInstance(typeof(SavedData));
    }

    /// <summary>
    /// Saves argument data into the argument slot.
    /// </summary>
    /// <param name="data">SavedData object to be stored.</param>
    /// <param name="slot">1, 2, or 3 (else error thrown).</param>
    public static void StoreDataAtSlot(in SavedData data, in int slot) {
        if (slot < 1 || slot > 3) throw new IndexOutOfRangeException();
        var fStream = File.Create(getDataPath + saveStr 
                                  + slot.ToString() + ".dat");
        var wrapper = new DataWrapper(data);
        new BinaryFormatter().Serialize(fStream, wrapper);
        fStream.Close();
    }

    /// <summary>
    /// Saves argument data into the argument slot as an async method.
    /// </summary>
    /// <param name="data">SavedData object to be stored.</param>
    /// <param name="slot">1, 2, or 3 (else error thrown).</param>
    public static IEnumerator StoreDataAsyncAtSlot(SavedData data, int slot) {
        if (slot < 1 || slot > 3) throw new IndexOutOfRangeException();
        var wrapper = new DataWrapper(data);
        var fStream = File.Create(getDataPath + saveStr 
                                  + slot.ToString() + ".dat");
        var serialization = Task.Run(() => new BinaryFormatter()
                                .Serialize(fStream, wrapper));
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
        string filePath = getDataPath + saveStr 
                          + slot.ToString() + ".dat";
        if (File.Exists(filePath)) {
            File.Delete(filePath);
        }
        var fStream = File.OpenWrite(filePath);
        var wrapper = new DataWrapper((SavedData)ScriptableObject
                                    .CreateInstance(typeof(SavedData)));
        new BinaryFormatter().Serialize(fStream, wrapper); // not writable? // or nah?
        fStream.Close();
    }

    /// <summary>
    /// Short little function to immediately get the currently used data.
    /// USE WITH CAUTION (sharing violation errors could be thrown).
    /// </summary>
    /// <returns>The SavedData of the game currently loaded.</returns>
    public static SavedData GetCurrentlyLoadedData() {
        return GetDataStoredAt(currentSaveSlot);
    }

    /* PUBLIC FUNCTIONS TO MANIPULATE TIMEPLAYED TIMESPAN VARIABLE */
    /// <summary>
    /// Start timer for this current playthrough, make sure this is called on
    /// loading a save slot.
    /// </summary>
    public static void StartTimer() {
        if (startPlayTime == null) startPlayTime = DateTime.Now;
        else Debug.Log("StartTimer(): no action taken, timer already running");
    }

    /// <summary>
    /// Upon quitting, cut off the current play time and update timePlayed.
    /// </summary>
    public static IEnumerator CutCurrentPlayTime(SavedData data) {
        if (startPlayTime == null)
            Debug.LogError("SavedData.cs: play timer has not yet been set.");
        else {
            //data.timePlayed is not set to an instance of an object
            data.timePlayed += DateTime.Now - (DateTime)startPlayTime;
            startPlayTime = null;
        }
        var task = Task.Run(() => StoreDataAtSlot(data, currentSaveSlot));
        while (!task.IsCompleted) {
            Debug.Log("Saving...");
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }

    /// <summary>
    /// Retrieve timePlayed with startPlayTime taken into account.
    /// </summary>
    /// <returns>Current total time playing.</returns>
    public TimeSpan GetCurrentPlayTime() {
        if (startPlayTime == null) 
            return timePlayed;
        else
            return timePlayed + ((DateTime)startPlayTime - DateTime.Now);
    }

    public bool IsNewInstance() {
        return (timePlayed == TimeSpan.Zero && currentLevel == 1);
    }

}

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

    public SerializableQuiver() {
        loadout = new int[,] { { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 } };
        equipped = 0;
    }

    /// <summary>
    /// Construction based on an Quiver parameter
    /// </summary>
    /// <param name="serialize">Quiver to copy values from.</param>
    public SerializableQuiver(in Quiver serialize) {
        loadout  = serialize.getLoadout;
        equipped = serialize.getEquipped;
    }

    /// <summary>
    /// Serialize a Quiver based on implicitly passing Quiver's variables as
    /// parameters.
    /// </summary>
    public SerializableQuiver(in int[,] load, in int equip) {
        loadout  = load;
        equipped = equip;
    }
}