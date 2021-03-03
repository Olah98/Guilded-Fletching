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
    public Button[] saveSlotButtons = new Button[3];
    public Button mainMenuButton, loadSaveButton, cancelSelectButton;
    public Text currentLevelText, timePlayedText, totalArrowsShotText;
    [Header("UI Animation Values")]
    [Range(0.1f, 2.0f)]
    public float uiSpeed = 1f;

    private int slotSelected = -1;
    private SavedData[] dataArr = new SavedData[3];
    private TimeSpan timeSpanOfSlot = new TimeSpan(0, 0, 0);
    private const string currentLevelTextPrefix    = "Current Level: ";
    private const string timePlayedTextPrefix      = "Time Played: ";
    private const string totalArrowsShotTextPrefix = "Total Arrows Shot: ";
    
    private void Start() {
        for (int i = 0; i < 3; ++i) {
            string saveSlotTitle = "#" + (i + 1).ToString() + ": ";
            dataArr[i] = SavedData.GetDataStoredAt(i + 1);
            // only change text if there's no data available
            if (dataArr[i].saveName != String.Empty) {
                saveSlotButtons[i].GetComponentInChildren<Text>().text 
                    = saveSlotTitle;
            }
        }
        // hide on intialization
        EnabledShowButtonsOnSelections(false);
    }

    /// <summary>
    /// Change scene back to Main Menu
    /// </summary>
    public void GoToMainMenu() {
        slotSelected = -1;
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Called when initially selecting a save slot.
    /// </summary>
    /// <param name="slot">Save slot selected.</param>
    public void SelectDataSlot(int slot) {
        if (slotSelected != -1) return; //prevent double clicks

        slotSelected = ++slot;
        var selected = SavedData.GetDataStoredAt(slot);
        //display important stats
        currentLevelText.text = currentLevelTextPrefix + selected.currentLevel;
        timePlayedText.text = timePlayedTextPrefix + selected.timePlayed;
        string numOfArrowsStr = (selected.playerQuiver == null) ? "0" 
                            : GetTotalArrowsShot(selected.playerQuiver).ToString();
        totalArrowsShotText.text = totalArrowsShotTextPrefix + numOfArrowsStr;
        StartCoroutine(AnimateShowStatistics(true));
    }

    public void CancelSlotSelection() {
        StartCoroutine(AnimateShowStatistics(false));
    }

    //actually open data, load appropriate scene
    public void LoadSlotSelected() {
        var loading = SavedData.GetDataStoredAt(slotSelected);
        loading.StartTimer();
        //output in console that you're loading something
        //load scene
        // set player and position up
        //dont destroy on load until everything is set up
        print("loading slot: " + slotSelected);
    }

    /// <summary>
    /// Run fade-in or fade-out UI animation depending on the boolean 
    /// parameter.
    /// </summary>
    /// <param name="isShowing">True to enabled, false to disable</param>
    private IEnumerator AnimateShowStatistics(bool isShowing) {
        EnabledShowButtonsOnSelections(isShowing);
        Color statColor   = currentLevelText.color;
        Color sSColor     = saveSlotButtons[0].image.color;
        Color sSTextColor = saveSlotButtons[0]
                                  .GetComponentInChildren<Text>().color;
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
                if (i == slotSelected - 1) continue;

                saveSlotButtons[i].image.color = sSColor;
                saveSlotButtons[i].GetComponentInChildren<Text>()
                    .color = sSTextColor;
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
            slotSelected = -1; 
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
        loadSaveButton.gameObject.SetActive    (isShowing);
        cancelSelectButton.gameObject.SetActive(isShowing);
        mainMenuButton.gameObject.SetActive    (!isShowing);
    }

    // private helper statistic functions
    /// <summary>
    /// Take record of all arrows and return the sum as an unsigned integer.
    /// </summary>
    /// <param name="q">Quiver being calculated.</param>
    /// <returns>Total number of arrows fired.</returns>
    private uint GetTotalArrowsShot(Quiver q) {
        uint total = 0;
        string[] arrowStr = { "Standard", "Bramble", "Warp", "Airburst" };
        for (int i = 0; i < 4; ++i)
            total += (uint)q.GetArrowTypeShot(arrowStr[i]);
        return total;
    }
}