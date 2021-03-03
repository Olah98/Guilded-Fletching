/*
Author: Christian Mullins
Date: 03/01/2021
Summary: Scriptable object type save file that can store, retrieve, and edit 
    itself.
*/
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(
    fileName = "SavedData", 
    menuName = "Scriptable Objects/SavedData", 
    order = 0)]
public class SavedData : ScriptableObject {
    public string saveName = String.Empty;
    [Header("Player Values")]
    public int playerHealth = 100;
    public Quiver playerQuiver = null;
    [Header("Level Values")]
    //public int maxLevelReached = 1;           // use in later implementation
    public int currentLevel = 1;
    public Vector3 spawnPos = Vector3.zero;
    [Header("Option Values")]
    public float musicVol = 1f;
    public float soundFXVol = 1f;
    [Range(0.1f, 1f)]
    public float mouseSensitivity = 0.3f;
    public float baseFOV = 60f;                  // default Unity FOV
    [Header("Misc Values")]
    public TimeSpan timePlayed = new TimeSpan();

    [HideInInspector] public static int currentSaveSlot = -1;

    private static DateTime? startPlayTime;      // nullable
    private const string saveStr = "SAVE_SLOT_"; // concatenate 1, 2, or 3

    /* STATIC STORAGE AND RETRIEVAL METHODS */
    /// <summary>
    /// Returns data stored at a given slot.
    /// </summary>
    /// <param name="slot">1, 2, or 3 (else error thrown).</param>
    /// <returns>Data stored at the argument slot.</returns>
    public static SavedData GetDataStoredAt(in int slot) {
        if (slot < 1 || slot > 3) throw new IndexOutOfRangeException();
        string filePath = Application.persistentDataPath + saveStr 
                          + slot.ToString() + ".dat";
        if (File.Exists(filePath)) {
            var fStream = File.OpenRead(filePath);
            var bf = new BinaryFormatter();
            var dataSlot = (SavedData) bf.Deserialize(fStream);
            fStream.Close();
            return dataSlot;
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
        var fStream = File.Create(Application.persistentDataPath + saveStr 
                                  + slot.ToString() + ".dat");
        new BinaryFormatter().Serialize(fStream, data);
        fStream.Close();
    }

    /// <summary>
    /// Revert a save slot back to empty constructor.
    /// </summary>
    /// <param name="slot">1, 2, or 3 (else error thrown).</param>
    public static void DeleteDataSlot(in int slot) {
        if (slot < 1 || slot > 3) throw new IndexOutOfRangeException();
        string filePath = Application.persistentDataPath + saveStr 
                          + slot.ToString() + ".dat";
        if (File.Exists(filePath)) {
            var fStream = File.OpenRead(filePath);
            var newData = (SavedData)ScriptableObject
                            .CreateInstance(typeof(SavedData));
            new BinaryFormatter().Serialize(fStream, newData);
            fStream.Close();
        }
    }

    /* PUBLIC FUNCTIONS TO MANIPULATE TIMEPLAYED TIMESPAN VARIABLE */
    /// <summary>
    /// Start timer for this current playthrough, make sure this is called on
    /// loading a save slot.
    /// </summary>
    public void StartTimer() {
        if (startPlayTime == null) startPlayTime = DateTime.Now;
        else Debug.Log("StartTimer(): no action taken, timer already running");
    }

    /// <summary>
    /// Upon quitting, cut off the current play time and update timePlayed.
    /// </summary>
    public void CutCurrentPlayTime() {
        if (startPlayTime == null) 
            Debug.LogError("SavedData.cs: play timer has not yet been set.");
        timePlayed += (DateTime)startPlayTime - DateTime.Now;
        startPlayTime = null;
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
}