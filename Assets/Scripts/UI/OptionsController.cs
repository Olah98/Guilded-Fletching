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
        -Fullscreen Toggle //By Warren
*/
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class OptionsController : MonoBehaviour {
    public struct Option {
        public Slider slider;
        public Text numStr;
    }
    // used to access specific optionGroup[] by index
    public enum OptionType {
        GraphicsQuality = 0, MouseSensitivity, BaseFOV, 
        MasterVol,           SoundFXVol,       AmbientVol
    }
    
    public Dropdown saveSelector;
    public Toggle fullScreen;
    [Tooltip("Graphics Quality\nMouseSensitivity\nBaseFOV\nMaster\nSoundFX\nAmbient")]
    public GameObject[] optionGroup = new GameObject[6];

    private Option[] _options = new Option[6];
    private SavedData _curData;
    //private int curSaveIndex = -1;
    private GameObject blackScreen; //By Warren
    private bool _isOptionsMenu { get { 
        return SceneManager.GetActiveScene().name == "Options"; 
    } }

    private void Start() {
        _curData = (_isOptionsMenu) 
                ? SavedData.GetDataStoredAt(saveSelector.value + 1)
                : SavedData.GetDataStoredAt(SavedData.currentSaveSlot);
        
        // take optionGroup GameObject, populate options[], and set text
        _options = new Option[optionGroup.Length];
        for (int i = 0; i < optionGroup.Length; ++i) {
            _options[i].numStr = optionGroup[i].transform.GetChild(1).GetComponent<Text>();
            _options[i].slider = optionGroup[i].GetComponentInChildren<Slider>();
        }
        fullScreen.GetComponent<Toggle>().isOn = Screen.fullScreen; //By Warren
        InitializeOptionUI();
        blackScreen = GameObject.FindGameObjectWithTag("ScreenShift"); //By Warren
    }

    private void OnEnable() {
        if (_curData != null) InitializeOptionUI();
    }
    #region UI
    /// <summary>
    /// Immediately set up UI values in the Options in one function.
    /// </summary>
    public void InitializeOptionUI() {
        //grab options data form the saved data
        //plug options data into the optionsUI array
        var curOptionsData = SavedData.GetStoredOptionsAt(SavedData.currentSaveSlot);
        // graphics quality
        _options[0].slider.value = curOptionsData.graphicsQuality;
        _options[0].numStr.text  = ((int)curOptionsData.graphicsQuality * 100f).ToString();
        // mouse sensitivity
        curOptionsData.mouseSensitivity 
            = Mathf.Clamp(curOptionsData.mouseSensitivity, 0.1f, 1.0f);
        _options[1].slider.value = curOptionsData.mouseSensitivity;
        _options[1].numStr.text  = ((int)(curOptionsData.mouseSensitivity * 100f)).ToString();
        // base fov(field of view)
        _options[2].slider.value = curOptionsData.baseFOV * 0.005f;
        _options[2].numStr.text  = ((int)curOptionsData.baseFOV).ToString();
        // master volume
        _options[3].slider.value = curOptionsData.masterVol;
        _options[3].numStr.text  = ((int)(curOptionsData.masterVol * 100f)).ToString();
        // sound FX volume
        _options[4].slider.value = curOptionsData.soundFXVol;
        _options[4].numStr.text  = ((int)(curOptionsData.soundFXVol * 100f)).ToString();
        // ambient/music volume
        _options[5].slider.value = curOptionsData.musicVol;
        _options[5].numStr.text  = ((int)(curOptionsData.musicVol * 100f)).ToString();
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
        if (_options[optionIndex].slider == null
            || _options[optionIndex].numStr == null) return;

        if (optionIndex == (int)OptionType.MouseSensitivity) {
            _options[1].numStr.text = ((int)(_options[1].slider.value * 100f)).ToString();
        }
        else if (optionIndex == (int)OptionType.BaseFOV) {
            float fovVal = Mathf.Clamp(
                            _options[optionIndex].slider.value * 200, 20f, 120f);
            // adjust value in real time
            if (!_isOptionsMenu) Camera.main.fieldOfView = fovVal;
            _options[optionIndex].numStr.text = ((int)fovVal).ToString();
        }
        else {
            _options[optionIndex].numStr.text 
                = ((int)(_options[optionIndex].slider.value * 100f)).ToString();

            // if this is in a game scene and this is the volume index update
            // volume in real time (volume is indexed as 3+)
            if (!_isOptionsMenu && optionIndex > 2) {
                SavedData.SetOptionsInScene(PackControllerOptions());
            }
        }
    }
    #endregion

    /// <summary>
    /// Set up save slot options to personalize options set up.
    /// </summary>
    public void SaveSlotDropDownSelection() {
        int saveSlot = (_isOptionsMenu) ? saveSelector.value + 1 : SavedData.currentSaveSlot;
        _curData = SavedData.GetDataStoredAt(saveSlot);
        _options[(int)OptionType.GraphicsQuality].slider.value  = _curData.graphicsQuality;
        _options[(int)OptionType.MouseSensitivity].slider.value = _curData.mouseSensitivity;
        _options[(int)OptionType.BaseFOV].slider.value          = _curData.baseFOV;
        _options[(int)OptionType.MasterVol].slider.value        = _curData.masterVol;
        _options[(int)OptionType.SoundFXVol].slider.value       = _curData.soundFXVol;
        _options[(int)OptionType.AmbientVol].slider.value       = _curData.musicVol;
        for (int i = 0; i < _options.Length; ++i) {
            _options[i].numStr.text = ((int)_options[i].slider.value * 100).ToString();
        }
    }

    /// <summary>
    /// Update values to the values of the curently used SavedData slot.
    /// </summary>
    public void SaveOptionsToCurrentData() {
        _curData.graphicsQuality  = _options[(int)OptionType.GraphicsQuality].slider.value;
        _curData.mouseSensitivity = _options[(int)OptionType.MouseSensitivity].slider.value;
        _curData.baseFOV          =  (float)Convert.ToDouble(_options[(int)OptionType.BaseFOV].numStr.text);
        _curData.masterVol        = _options[(int)OptionType.MasterVol].slider.value;
        _curData.soundFXVol       = _options[(int)OptionType.SoundFXVol].slider.value;
        _curData.musicVol         = _options[(int)OptionType.AmbientVol].slider.value;
        //update volume in real time
        var options = new OptionsData(_curData);
        SavedData.StoreOptionsAt(options, SavedData.currentSaveSlot);
        SavedData.SetOptionsInScene(options);
    }

    /// <summary>
    /// Take values of this class and pack as OptionsData.
    /// </summary>
    /// <returns>OptionsData form of this controller.</returns>
    public OptionsData PackControllerOptions() {
        var data = new OptionsData();
        data.graphicsQuality  = _options[0].slider.value;
        data.mouseSensitivity = _options[1].slider.value;
        data.baseFOV          = _options[2].slider.value;
        data.masterVol        = _options[3].slider.value;
        data.soundFXVol       = _options[4].slider.value;
        data.musicVol         = _options[5].slider.value;
        return data;
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

    public void loadScene(string level)
    {
        //SceneManager.LoadScene(level);
        StartCoroutine(LoadSceneCo(level));//By Warren
    }//loadScene

    /*
    * On Toggle Fullscreen - By Warren 
    * Toggles between windowed and fullscreen modes
    */
    public void OnToggle_Fullscreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }//OnToggle_Fullscreen

    /*
    * Delay - By Warren 
    * Calls a screen shift and waits for the change, if a target is available
    */
    public IEnumerator Delay()
    {
        if (blackScreen != null)
        {
            blackScreen.GetComponent<ScreenShift>().Change();
            yield return new WaitForSeconds(1f);
        }
    }//Delay

    /*
    * Load Scene Co - By Warren 
    * Asks for a delay for a screen fade before loading the target level/menu 
    */
    public IEnumerator LoadSceneCo(string level)
    {
        yield return Delay();
        SceneManager.LoadScene(level);
    }//LoadSceneCo
}//OptionsController
