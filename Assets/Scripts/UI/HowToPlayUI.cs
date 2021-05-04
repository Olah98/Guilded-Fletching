﻿/*
Author: Warren Rose II
Data: 05/02/2021
Summary: Overview of controls, set to update if keys are rebound.
    In the Main Menu Controls page, this allows rebinding of keys.
    A local copy of DisplayKeys is used to read and bind controls.
References: Using DapperDino's Tutorial https://youtu.be/dUCcZrPhwSo
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;
using TMPro;

public class HowToPlayUI : MonoBehaviour
{
    //Needed for general key bindings to text
    private DisplayKeys _displayKeys;

    //Simple Controls
    private string _jumpKey, _interactKey, _cancelKey;
    private string _fireKey, _zoomKey;

    //Controls with a main key and an alternate trigger
    private string _standardKey, _standardKey2, _brambleKey;
    private string _brambleKey2, _warpKey, _warpKey2, _airburstKey;
    private string _airburstKey2, _crouchKey, _crouchKey2;

    //[0] and [5] display "2D Axis" as that is the type of control
    private string _upKey, _downKey, _leftKey, _rightKey;
    private string _upKey2, _downKey2, _leftKey2, _rightKey2;


    //Specific to Controls UI
    public TMP_Text controlsText;
    public TMP_Text standText;
    public TMP_Text brambText;
    public TMP_Text warpText;
    public TMP_Text airText;
    public TMP_Text secondaryControlsText;
    private const string _firstBrambleLevel = "Four";
    private const string _firstWarpLevel = "Five";
    private const string _firstAirburstLevel = "Six";
    private bool _showSecondaryControls;

    //Specific to Main Menu Controls UI
    public TMP_Text titleText;
    public TMP_Text buttonText;
    public GameObject rebindBlock;
    public GameObject[] items = new GameObject[6];
    public TMP_Text[] refs = new TMP_Text[23];
    private bool _secondScreen;
    private string _firstTitle = "How To Play";
    private string _secondTitle = "Rebind Controls";
    private const int _rebindsLength = 23;

    //From DapperDino Tutorial
    private InputActionRebindingExtensions.RebindingOperation _rebindingOperation;

    /*
    * Awake
    * Creates local copy of DisplayKeys, before Start runs
    */
    void Awake()
    {
        //Needed for general key bindings to text
        _displayKeys = gameObject.AddComponent(typeof(DisplayKeys)) as DisplayKeys;
    }//Awake

    /*
     * Start
     * Preloads strings with the definitions provided by DisplayKeys
     * Fills out the TextMeshPro Text objects.
     */
    void Start()
    {
        UpdateStrings();
        UpdateText();
    }//Start

    /*
    * Toggle Menus
    * Turns on and off the visibilty of menu components
    * Asks for the strings to be updated across the UI
    * This only works on the Main Menu Controls page
    * Fortunately, it's called by a button that
    *   doesn't exist inside of the Pause UI Prefab
    */
    public void ToggleMenus()
    {
        for (int i = 0; i < items.Length; i++)
        {
            items[i].SetActive(!items[i].activeSelf);

        }

        _secondScreen = !_secondScreen;

        if (_secondScreen)
        {
            titleText.text = _secondTitle;
            buttonText.text = _firstTitle;
        }
        else
        {
            titleText.text = _firstTitle;
            buttonText.text = _secondTitle;
        }

        UpdateStrings();
        UpdateText();
    }//ToggleMenus

    /*
    * Update Strings
    * Preloads strings with the definitions provided by DisplayKeys
    */
    public void UpdateStrings()
    {
        //Needed for general key bindings to text

        //Simple Controls
        _jumpKey = _displayKeys.FirstKey("Jump");
        _interactKey = _displayKeys.FirstKey("Interact");
        _cancelKey = _displayKeys.FirstKey("Cancel");
        _fireKey = _displayKeys.FirstKey("Fire");
        _zoomKey = _displayKeys.FirstKey("Zoom");

        //Controls with a main key and an alternate trigger
        _standardKey = _displayKeys.FirstKey("Standard");
        _standardKey2 = _displayKeys.SecondKey("Standard");
        _brambleKey = _displayKeys.FirstKey("Bramble");
        _brambleKey2 = _displayKeys.SecondKey("Bramble");
        _warpKey = _displayKeys.FirstKey("Warp");
        _warpKey2 = _displayKeys.SecondKey("Warp");
        _airburstKey = _displayKeys.FirstKey("Airburst");
        _airburstKey2 = _displayKeys.SecondKey("Airburst");
        _crouchKey = _displayKeys.FirstKey("Crouch");
        _crouchKey2 = _displayKeys.SecondKey("Crouch");

        //[0] and [5] display "2D Axis" as that is the type of control
        _upKey = _displayKeys.ByPart("Movement", 1);
        _downKey = _displayKeys.ByPart("Movement", 2);
        _leftKey = _displayKeys.ByPart("Movement", 3);
        _rightKey = _displayKeys.ByPart("Movement", 4);
        _upKey2 = _displayKeys.ByPart("Movement", 6);
        _downKey2 = _displayKeys.ByPart("Movement", 7);
        _leftKey2 = _displayKeys.ByPart("Movement", 8);
        _rightKey2 = _displayKeys.ByPart("Movement", 9);
    }//UpdateStrings

    /*
    * Update Text
    * Fills out the TextMeshPro Text objects.
    */
    public void UpdateText()
    {
        //Specific to Controls UI

        if (!_secondScreen)
        {
            string _controlsDescription = "";
            string _changeVisibleDescription = "";
            if (!_showSecondaryControls)
            {
                _controlsDescription = "<b>To Move:</b> " + _upKey + ", " +
                    _downKey + ", " + _leftKey + ", "
                    + _rightKey + "\n" + "<b>Interact:</b> " + _interactKey + "\n" +
                    "<b>Hold to Fire:</b> " + _fireKey + "\n" + "<b>Zoom:</b> " + _zoomKey + "\n" +
                    "<b>Jump:</b> " + _jumpKey + "\n" + "<b>Pause Game:</b> " + _cancelKey + "\n" +
                    "<b>Equip Standard Arrows:</b> " + _standardKey + "\n" +
                    "<b>Equip Bramble Arrows:</b> " + _brambleKey + "\n" +
                    "<b>Equip Warp Arrows:</b> " + _warpKey + "\n" +
                    "<b>Equip Airburst Arrows:</b> " + _airburstKey + "\n" +
                    "<b>Crouch:</b> " + _crouchKey;

                _changeVisibleDescription = "Show Alternate Keys";
            }
            else
            {
                _controlsDescription = "<b>To Move:</b> " + _upKey2 + ", " +
                    _downKey2 + ", " + _leftKey2 + ", "
                    + _rightKey2 + "\n" + "<b>Interact:</b> " + _interactKey + "\n" +
                    "<b>Hold to Fire:</b> " + _fireKey + "\n" + "<b>Zoom:</b> " + _zoomKey + "\n" +
                    "<b>Jump:</b> " + _jumpKey + "\n" + "<b>Pause Game:</b> " + _cancelKey + "\n" +
                    "<b>Equip Standard Arrows:</b> " + _standardKey2 + "\n" +
                    "<b>Equip Bramble Arrows:</b> " + _brambleKey2 + "\n" +
                    "<b>Equip Warp Arrows:</b> " + _warpKey2 + "\n" +
                    "<b>Equip Airburst Arrows:</b> " + _airburstKey2 + "\n" +
                    "<b>Crouch:</b> " + _crouchKey2;

                _changeVisibleDescription = "Show Primary Keys";
            }
        

            controlsText.text = _controlsDescription;
            secondaryControlsText.text = _changeVisibleDescription;
            standText.text =
                "Shoot targets with the Standard Arrow, unlocked in Level One.";
            brambText.text =
                "Bind objects with the Bramble Arrow, unlocked in Level "
                + _firstBrambleLevel + ".";
            warpText.text =
                "Warp around the world with the Warp Arrow, unlocked in Level "
                + _firstWarpLevel + ".";
            airText.text =
                "Knock over and blow away objects with the Airburst Arrow, unlocked in Level "
                + _firstAirburstLevel + ".";
        }
        else
        {
            refs[0].text = _upKey; refs[1].text = _downKey;
            refs[2].text = _leftKey; refs[3].text = _rightKey;
            refs[4].text = _upKey2; refs[5].text = _downKey2;
            refs[6].text = _leftKey2; refs[7].text = _rightKey2;

            refs[8].text = _standardKey; refs[9].text = _brambleKey;
            refs[10].text = _warpKey; refs[11].text = _airburstKey;
            refs[12].text = _standardKey2; refs[13].text = _brambleKey2;
            refs[14].text = _warpKey2; refs[15].text = _airburstKey2;

            refs[16].text = _jumpKey; refs[17].text = _interactKey;
            refs[18].text = _cancelKey; refs[19].text = _fireKey;
            refs[20].text = _zoomKey;
            refs[21].text = _crouchKey; refs[22].text = _crouchKey2;
        }
    }//UpdateText

    /*
    * Rebind Key
    * Toggles on reblocker, rebinds key, allocates memory to rebinder
    */
    public void RebindKey(int actionIndex)
    {
        //Specific to Controls UI
        //InputBinding boundInput = _displayKeys.BindingsArrayLocation(actionIndex);
        InputAction actedInput = _displayKeys.ActionsArrayLocation(actionIndex);
        int boundInput = _displayKeys.BindingsArrayLocation(actionIndex);
        rebindBlock.SetActive(true);

        //From DapperDino Tutorial
        _rebindingOperation =
            actedInput.PerformInteractiveRebinding(boundInput)
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation => RebindComplete())
            .Start();
    }//RebindKey

    /*
    * Rebind Complete
    * Toggles off blocker, deletes rebinder from memory
    */
    public void RebindComplete()
    {
        //Specific to Controls UI
        rebindBlock.SetActive(false);
        UpdateStrings();
        UpdateText();
        _displayKeys.SaveKeys();

        //From DapperDino Tutorial
        _rebindingOperation.Dispose();
    }//RebindComplete

    /*
    * Reset All Keys
    * Removes overrides from all keys
    */
    public void ResetAllKeys()
    {
        //Specific to Controls UI
        for (int i = 0; i < _rebindsLength; i++)
        {
            InputAction actedInput = _displayKeys.ActionsArrayLocation(i);
            int boundInput = _displayKeys.BindingsArrayLocation(i);
            actedInput.RemoveBindingOverride(boundInput);
        }

        UpdateStrings();
        UpdateText();
        _displayKeys.SaveKeys();
    }//ResetAllKeys

    /*
    * Reset Key
    * Removes overrides from specific key
    */
    public void ResetKey(int actionIndex)
    {
        //Specific to Controls UI
        InputAction actedInput = _displayKeys.ActionsArrayLocation(actionIndex);
        int boundInput = _displayKeys.BindingsArrayLocation(actionIndex);
        actedInput.RemoveBindingOverride(boundInput);

        UpdateStrings();
        UpdateText();
        _displayKeys.SaveKeys();
    }//ResetAllKeys

    /*
    * Toggle Secondary Controls View
    * Switches in and out the view of the primary and secondary
    * control set ups.
    */
    public void ToggleSecondaryControlsView()
    {
        _showSecondaryControls = !_showSecondaryControls;
        UpdateStrings();
        UpdateText();
    }//ToggleSecondaryControlsView
}//HowToPlayUI