/*
Author: Christian Mullins
Date: 03/05/2021
Summary: Controls options in the Option scene and how it handles data
    transfering.
    Values handled:
        -Graphics Quality Slider
        -Mouse Sensitivity
        -Base FOV
        -Master Volume
        -Sound FX Volume
        -Ambient(Music) Volume
*/
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using System;

public class OptionsController : MonoBehaviour {
    public struct Option {
        public Slider slider;
        public Text numStr;
    }
    // used to access specific optionGroup[] by index
    public enum OptionType {
        GraphicsQuality = 0, MouseSensitivity = 1, BaseFOV = 2, 
        MasterVol = 3,       SoundFXVol = 4,       AmbientVol = 5
    }
    
    public Dropdown saveSelector;
    [Tooltip("Graphics Quality\nMouseSensitivity\nBaseFOV\nMaster\nSoundFX\nAmbient")]
    public GameObject[] optionGroup = new GameObject[6];

    private Option[] options = new Option[6];
    private SavedData curData;
    //private int curSaveIndex = -1;
    private bool isOptionsMenu { get { 
        return SceneManager.GetActiveScene().name == "Options"; 
    } }

    private void Start() {
        curData = (isOptionsMenu) 
                ? SavedData.GetDataStoredAt(saveSelector.value + 1)
                : SavedData.GetDataStoredAt(SavedData.currentSaveSlot);
        
        // take optionGroup GameObject, populate options[], and set text
        options = new Option[optionGroup.Length];
        for (int i = 0; i < optionGroup.Length; ++i) {
            options[i].numStr = optionGroup[i].transform.GetChild(1).GetComponent<Text>();
            options[i].slider = optionGroup[i].GetComponentInChildren<Slider>();
        }
        InitializeOptionUI();
    }

    /// <summary>
    /// Immediately set up UI values in the Options in one function.
    /// </summary>
    public void InitializeOptionUI() {
        // graphics quality
        options[0].slider.value = curData.graphicsQuality;
        options[0].numStr.text  = ((int)curData.graphicsQuality * 100).ToString();
        // mouse sensitivity
        curData.mouseSensitivity = Mathf.Clamp(curData.mouseSensitivity, 0.1f, 1.0f);
        options[1].slider.value = curData.mouseSensitivity;
        options[1].numStr.text  = ((int)(curData.mouseSensitivity * 100)).ToString();
        // base fov(field of view)
        options[2].slider.value = curData.baseFOV * 0.005f;
        options[2].numStr.text  = ((int)curData.baseFOV).ToString();
        // master volume
        options[3].slider.value = curData.masterVol;
        options[3].numStr.text  = ((int)curData.masterVol * 100).ToString();
        // sound FX volume
        options[4].slider.value = curData.soundFXVol;
        options[4].numStr.text  = ((int)curData.soundFXVol * 100).ToString();
        // ambient/music volume
        options[5].slider.value = curData.musicVol;
        options[5].numStr.text  = ((int)curData.musicVol * 100).ToString();
    }

    /// <summary>
    /// Exit out of Options Menu with logic handling for the context of options.
    /// </summary>
    public void GoBack() {
        SaveOptionsToCurrentData();
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Called each time a value in the options is adjusted.
    /// </summary>
    /// <param name="optionIndex">Unity doesn't like enums, so cast to int.</param>
    public void OnSlide_ChangeValueFor(int optionIndex) {
        // guard clause because this function is somehow called on start
        if (options[optionIndex].slider == null
            || options[optionIndex].numStr == null) return;

        if (optionIndex == (int)OptionType.MouseSensitivity) {
            float newVal = options[1].slider.value;
            newVal = Mathf.Clamp(newVal, 0.1f, 1.0f);
            options[1].slider.value = newVal;
            options[1].numStr.text  = ((int)(newVal * 100)).ToString();
        }
        if (optionIndex == (int)OptionType.BaseFOV) {
            float fovVal = Mathf.Clamp(
                            options[optionIndex].slider.value * 200,
                            20f, 
                            120f);
            // adjust value in real time
            if (!isOptionsMenu) Camera.main.fieldOfView = fovVal;
            options[optionIndex].numStr.text = ((int)fovVal).ToString();
        }
        else {
            options[optionIndex].numStr.text 
            = ((int)options[optionIndex].slider.value * 100).ToString();
        }
    }

    /// <summary>
    /// Update class value to the currently used SavedData.
    /// </summary>
    public void UpdateValuesToSave() {
        //update to curData
        options[(int)OptionType.MouseSensitivity].slider.value = curData.mouseSensitivity;
        options[(int)OptionType.BaseFOV].slider.value          = curData.baseFOV;
        options[(int)OptionType.MasterVol].slider.value        = curData.masterVol;
        options[(int)OptionType.SoundFXVol].slider.value       = curData.soundFXVol;
        options[(int)OptionType.AmbientVol].slider.value       = curData.musicVol;
        for (int i = 0; i < options.Length; ++i) {
            options[i].numStr.text = ((int)options[i].slider.value * 100).ToString();
        }
    }

    /// <summary>
    /// Set up save slot options to personalize options set up.
    /// </summary>
    public void SaveSlotDropDownSelection() {
        int saveSlot = (isOptionsMenu) ? saveSelector.value + 1 : SavedData.currentSaveSlot;
        curData = SavedData.GetDataStoredAt(saveSlot);
        UpdateValuesToSave();
    }

    /// <summary>
    /// Update values to the values of the curently used SavedData slot.
    /// </summary>
    public void SaveOptionsToCurrentData() {
        curData.graphicsQuality  = options[(int)OptionType.GraphicsQuality].slider.value;
        curData.mouseSensitivity = options[(int)OptionType.MouseSensitivity].slider.value;
        curData.baseFOV          = (float)Convert.ToDouble(
                                    options[(int)OptionType.BaseFOV].numStr.text);
        curData.masterVol        = options[(int)OptionType.MasterVol].slider.value;
        curData.soundFXVol       = options[(int)OptionType.SoundFXVol].slider.value;
        curData.musicVol         = options[(int)OptionType.AmbientVol].slider.value;
        // update values for the player
        Camera.main.transform.GetComponentInParent<Character>()
            .UpdateCharacterToSaveData(curData);
    }

    /// <summary>
    /// Retrieve option index value in a parameter save data.
    /// </summary>
    /// <param name="type">OptionType to look for.</param>
    /// <param name="data">SavedData we're looking up.</param>
    /// <returns></returns>
    private float GrabDataByOptionIndex(in OptionType type, in SavedData data) {
        switch (type) {
            case OptionType.GraphicsQuality:  return data.graphicsQuality;
            case OptionType.MouseSensitivity: return data.mouseSensitivity;
            case OptionType.BaseFOV:          return data.baseFOV;
            case OptionType.MasterVol:        return data.masterVol;
            case OptionType.SoundFXVol:       return data.soundFXVol;
            case OptionType.AmbientVol:       return data.musicVol;
        }
        throw new System.IndexOutOfRangeException();
    }
}
