/*
Author: Christian Mullins
Date: 03/01/2021
Summary: Handles everything in the save select screen from animation to
    interactions with serialized data retrieval.
*/
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// script must be attached to Canvas object
[RequireComponent(typeof(Canvas))]
public class LoadGameController : MonoBehaviour {
    private enum ButtonInput {
        Save1, Save2, Save3, MainMenu, Load, Back, Delete, SaveNameAndLoad
    }
    [Header("Check buttons[] Tooltip for order")]
    [Tooltip("Save 1-3\nMainMenu\nLoad\nBack\nDelete\nSaveNameAndLoad")]
    public Button[] buttons = new Button[8];
    public GameObject inputFieldGroup;
    public Text currentLevelText, timePlayedText, totalArrowsShotText;
    [Header("UI Animation Values")]
    [Range(0.1f, 2.0f)]
    public float uiSpeed = 1f;

    private SavedData[] dataArr = new SavedData[3];
    private TimeSpan timeSpanOfSlot = new TimeSpan(0, 0, 0);
    private const string currentLevelTextPrefix    = "Current Level: ";
    private const string timePlayedTextPrefix      = "Time Played: ";
    private const string totalArrowsShotTextPrefix = "Total Arrows Shot: ";
    private Scene loadGameScene;
    
    private void Start() {
        for (int i = 0; i < 3; ++i) {
            dataArr[i] = SavedData.GetDataStoredAt(i + 1);
        }
        RefreshSaveNames();
        // hide on intialization
        EnabledShowButtonsOnSelections(false);
        inputFieldGroup.SetActive(false);
        loadGameScene = SceneManager.GetActiveScene();
    }

    /* PUBLIC BUTTON FUNCTIONS FOR UI */
    /// <summary>
    /// Change scene back to Main Menu
    /// </summary>
    public void GoToMainMenu() {
        SavedData.currentSaveSlot = -1;
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Called when initially selecting a save slot.
    /// </summary>
    /// <param name="slot">Save slot selected.</param>
    public void SelectDataSlot(int slot) {
        if (SavedData.currentSaveSlot != -1) return; //prevent double clicks

        SavedData.currentSaveSlot = ++slot;
        var selected = SavedData.GetDataStoredAt(slot);
        //display important stats
        currentLevelText.text    = currentLevelTextPrefix + selected.currentLevel;
        timePlayedText.text      = timePlayedTextPrefix + selected.timePlayed.ToString(@"hh\:mm\:ss");
        string numOfArrowsStr    = (selected.isNewInstance) ? "0" :
                                    GetTotalArrowsShot(selected.s_Quiver).ToString();
        totalArrowsShotText.text = totalArrowsShotTextPrefix + numOfArrowsStr;
        StartCoroutine(AnimateShowStatistics(true));
    }

    /// <summary>
    /// Used whenever "Back" is selected in UI or we want to return to original
    /// Load Game layout.
    /// </summary>
    public void CancelSlotSelection() {
        StartCoroutine(AnimateShowStatistics(false));
        SavedData.currentSaveSlot = -1;
        inputFieldGroup.SetActive(false);
    }

    //actually open data, load appropriate scene
    public void LoadSlotSelected() {
        // disable this button if setting the saveName
        if (inputFieldGroup.activeInHierarchy) return;

        var load = SavedData.GetDataStoredAt(SavedData.currentSaveSlot);
        if (load.saveName == String.Empty && load.timePlayed.Seconds == 0) {
            // throw up input box
            inputFieldGroup.SetActive(true);
        }
        else {
            //load.StartTimer();
            print("loading slot: " + SavedData.currentSaveSlot);
            // load scene
            int curLeveIndex = GetCurrentLevelIndex(load.currentLevel);
            StartCoroutine(LoadAndMigrateData(curLeveIndex, SavedData.GetCurrentlyLoadedData()));
            // set player and position upon scene load
            // dont destroy on load until everything is set up
        }
    }

    /// <summary>
    /// Button for Play! once the player enters their save name.
    /// </summary>
    public void SaveNameForData() {
        var name = inputFieldGroup.GetComponentInChildren<InputField>().text;
        //only return a non empty name
        if (name == String.Empty) return;
        // get currently used data
        var curData = SavedData.GetDataStoredAt(SavedData.currentSaveSlot);

        // set relavent values
        curData.saveName = name;
        print("savedName: " + name);
        inputFieldGroup.SetActive(false);
        // now load game (may need to "DontDestroyOnLoad this)
        //StartCoroutine(SavedData.StoreDataAsyncAtSlot(curData, SavedData.currentSaveSlot));
        // get player's quiver in scene 
        // copy quiver to saved data

        // store saved data
        int curLevelIndex = GetCurrentLevelIndex(curData.currentLevel);
        StartCoroutine(LoadAndMigrateData(curLevelIndex, curData));
    }

    /// <summary>
    /// Function for the Delete button.
    /// </summary>
    public void DeleteSelectedDataSlot() {
        SavedData.DeleteDataSlot(SavedData.currentSaveSlot);
        SavedData.currentSaveSlot = -1;
        CancelSlotSelection();
        RefreshSaveNames();
    }

    /* PRIVATE UI MOVEMENT FUNCTIONS */
    /// <summary>
    /// Run fade-in or fade-out UI animation depending on the boolean 
    /// parameter.
    /// </summary>
    /// <param name="isShowing">True to enabled, false to disable</param>
    private IEnumerator AnimateShowStatistics(bool isShowing) {
        EnabledShowButtonsOnSelections(isShowing);
        Color statColor   = currentLevelText.color;
        Color sSColor     = buttons[0].image.color;
        Color sSTextColor = buttons[0].GetComponentInChildren<Text>().color;
        // swap direction of looping values
        bool  isLooping;
        float incrementVal = (isShowing) ? 0.1f : -0.1f;
        float opacity      = (isShowing) ? 0f   : 1f;
        float endBound     = (isShowing) ? 1f   : 0f;
        // start animating
        do {
            sSColor.a = 1f - opacity;
            sSTextColor.a = 1f - opacity;
            for (int i = 0; i < 3; ++i) {
                // don't adjust a slot if not neccessary*****
                if (i == SavedData.currentSaveSlot - 1) continue;
                buttons[i].image.color = sSColor;
                buttons[i].GetComponentInChildren<Text>().color = sSTextColor;
            }
            // fade stat text in/out
            statColor.a = (isShowing) ? opacity : 1f - opacity;
            currentLevelText.color    = statColor;
            timePlayedText.color      = statColor;
            totalArrowsShotText.color = statColor;
            // increment and evaluate
            isLooping = (isShowing) ? opacity <= endBound 
                                    : opacity >= endBound;
            opacity += incrementVal;
            yield return new WaitForSeconds(uiSpeed * Time.deltaTime);
        } while (isLooping);

        if (!isShowing) { 
            EnabledShowButtonsOnSelections(false); 
            SavedData.currentSaveSlot = -1; 
        }
        yield return null;
    }

    /// <summary>
    /// Function to set certain 
    /// </summary>
    /// <param name="isShowing">Determines if UI is enabled or disabled</param>
    private void EnabledShowButtonsOnSelections(bool isShowing) {
        currentLevelText.enabled    = isShowing;
        timePlayedText.enabled      = isShowing;
        totalArrowsShotText.enabled = isShowing;
        // !MainMenu, Load, Back, Delete
        for (int i = 3; i < 8; ++i) {
            bool setTo = (i == 3)  ? !isShowing : isShowing; // 3 is MainMenu
            buttons[i].gameObject.SetActive(setTo);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void RefreshSaveNames() {
        for (int i = 0; i < 3; ++i) {
            // only change text if it's an existing sav
            string saveSlotTitle = "#" + (i + 1).ToString() + ": ";
            dataArr[i] = SavedData.GetDataStoredAt(i + 1);
            if (dataArr[i].isNewInstance)  {
                buttons[i].GetComponentInChildren<Text>()
                        .text = "Save Slot #" + (i + 1).ToString();
            }
            else {
                buttons[i].GetComponentInChildren<Text>()
                        .text = saveSlotTitle + dataArr[i].saveName;
            }
        }
    }

    /* PRIVATE HELPER STATISTIC FUNCTIONS */
    /// <summary>
    /// Take record of all arrows and return the sum as an unsigned integer.
    /// </summary>
    /// <param name="sQ">SerializableQuiver being calculated.</param>
    /// <returns>Total number of arrows fired.</returns>
    private uint GetTotalArrowsShot(in SerializableQuiver sQ) {
        uint total = 0;
        try {
            string[] arrowStr = { "Standard", "Bramble", "Warp", "Airburst" };
            for (int i = 0; i < 4; ++i)
                total += (uint)sQ.loadout[i, 1];
        } catch (NullReferenceException) {}
        return total;
    }

    /* PRIVATE SCENE MIGRATION FUNCTIONS */

    private int GetCurrentLevelIndex(int levelNum) {
        /*
        WILL BE UPDATED AS MORE LEVELS ARE IMPLEMENTED
        Future Implementation #1:
        switch (levelNum) {
            case 1:
        }
        Future Implementation #2
        return levelNum + pad;
        */
        // NOTE: function must pass the index, not the Scene itself (it's null)
        string path = "Assets/Scenes/Digital Prototype 1/Digital Enviroment.unity";
        int index = SceneUtility.GetBuildIndexByScenePath(path);
        print("sceneyscene: " + index);
        SceneUtility.GetScenePathByBuildIndex(3);
        if (SceneUtility.GetScenePathByBuildIndex(index) != path)
            Debug.LogWarning("Error for Christian:" 
                             +" update scenes for level loading");
        return index;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="levelIndex">Index of the current level to play.</param>
    /// <param name="data">Player data in use.</param>
    private IEnumerator LoadAndMigrateData(int levelIndex, SavedData data) {
        var levelOp = SceneManager.LoadSceneAsync(
                                        GetCurrentLevelIndex(levelIndex),
                                        LoadSceneMode.Additive);
        while (!levelOp.isDone) {
            print("Loading Level...");
            yield return new WaitForEndOfFrame();
        }

        // execute data transferal
        // fill SavedData if this a new game else fill the GameObject
        GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
        // new game
        if (data.isNewInstance) {
            // initialize values and save
            data.s_Vector3 = new SerializableVector3(
                                    playerGO.transform.position + Vector3.up);
            data.s_Quiver = new SerializableQuiver(
                                    playerGO.GetComponent<Quiver>());
        }
        // existing game
        //else
        playerGO.GetComponent<Character>().UpdateCharacterToSaveData(data);
        var curData = playerGO.GetComponent<Character>().UpdateAndGetSaveData();
        SavedData.StoreDataAtSlot(curData, SavedData.currentSaveSlot);
        SavedData.StartTimer();
        // finally unload the load scene
        levelOp = SceneManager.UnloadSceneAsync(loadGameScene);
        while (!levelOp.isDone) {
            print("Unloading load scene...");
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
}