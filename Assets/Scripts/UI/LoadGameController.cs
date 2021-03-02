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

// object must be attached to Canvas object
[RequireComponent(typeof(Canvas))]
public class LoadGameController : MonoBehaviour {
    public Button[] saveSlotButtons = new Button[3];
    public Button mainMenuButton, loadSaveButton, cancelSelectButton;
    public Text currentLevelText, timePlayedText, totalArrowsShotText;
    [Header("UI Animation Values")]
    [Range(0.1f, 2.0f)]
    public float uiSpeed = 1f;
    [Range(0.1f, 2.0f)]
    public float uiOpacity = 1f;

    private int slotSelected = -1;
    private SavedData[] dataArr = new SavedData[3];
    private TimeSpan timeSpanOfSlot = new TimeSpan(0, 0, 0);
    private const string currentLevelTextPrefix = "Current Level: ";
    private const string timePlayedTextPrefix = "Time Played: ";
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
    }

    // go back to main menu
    public void GoToMainMenu() {
        SceneManager.LoadScene("MainMenu");
        slotSelected = -1;
    }

    // button is selected, so start animation and fill appropriate variables
    public void SelectDataSlot(int slot) {
        slotSelected = slot;
        var selected = SavedData.GetDataStoredAt(slot);
        //display important stats
        currentLevelText.text = currentLevelTextPrefix + selected.currentLevel;
        timePlayedText.text = timePlayedTextPrefix + selected.timePlayed;
        totalArrowsShotText.text = totalArrowsShotTextPrefix
                                 + GetTotalArrowsShot(selected.playerQuiver);
        StartCoroutine(AnimateShowStatistics(true));
    }

    public void CancelSlotSelection() {
        StartCoroutine(AnimateShowStatistics(false));
        slotSelected = -1; // set at the end
    }

    //actually open data, load appropriate scene
    public void LoadSaveSlotAt() {
        var loading = SavedData.GetDataStoredAt(slotSelected);
        //output in console that you're loading something
        //load scene
        // set player and position up
        //dont destroy on load until everything is set up
        print("loading slot: " + slotSelected);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="isShowing"></param>
    /// <returns></returns>
    private IEnumerator AnimateShowStatistics(bool isShowing) {
        Color statColor = currentLevelText.color;
        Color sSColor = saveSlotButtons[0].image.color;
        Color sSTextColor = saveSlotButtons[0]
                                  .GetComponentInChildren<Text>().color;
        // swap direction of opacity incrementation
        float incrementVal     = (isShowing) ? 0.1f : -0.1f;
        float inverseIncrement = (isShowing) ? -1f  : 1f;
        for (float opacity = 0f; opacity <= 1.0f; opacity += incrementVal) {
            // fade unselected buttons out
            sSColor.a = opacity + inverseIncrement;
            sSTextColor.a = opacity + inverseIncrement;
            for (int i = 0; i < 3; ++i) {
                if (i == slotSelected) continue;
                saveSlotButtons[i].image.color = sSColor;
                saveSlotButtons[i].GetComponentInChildren<Text>()
                    .color = sSTextColor;
            }
            // fade stat text in
            statColor.a = (isShowing) ? opacity : 1f - opacity;
            currentLevelText.color = statColor;
            timePlayedText.color = statColor;
            totalArrowsShotText.color = statColor;
            yield return new WaitForSeconds(0.1f);
        }
        yield return null;
    }

    // private helper statistic functions
    private uint GetTotalArrowsShot(Quiver q) {
        uint total = 0;
        string[] arrowStr = { "Standard", "Bramble", "Warp", "Airburst" };
        for (int i = 0; i < 4; ++i)
            total += (uint)q.GetArrowTypeShot(arrowStr[i]);
        return total;
    }
}
