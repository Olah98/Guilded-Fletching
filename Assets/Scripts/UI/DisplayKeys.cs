﻿/*
Author: Warren Rose II
Data: 05/02/2021
Summary: Interprets the keys bound to various actions as strings.
    Also used to save rebound keys.
Referencing: DapperDino's UI Tutorial https://youtu.be/dUCcZrPhwSo
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DisplayKeys : MonoBehaviour
{
    //From DapperDino Tutorial
    private const string _rebindsKey = "rebinds";
    private const int _rebindsLength = 23;

    private Controls _controls;
    private InputAction[] _inputs = new InputAction[_rebindsLength];
    private int[] _bindings = new int[_rebindsLength];

    /*
    * Awake
    * Creates local copy of a Controls object
    */
    private void Awake()
    {
        _controls = new Controls();

        //From DapperDino Tutorial
        string rebinds = PlayerPrefs.GetString(_rebindsKey, string.Empty);

        if (string.IsNullOrEmpty(rebinds)) { return; }

        _controls.LoadBindingOverridesFromJson(rebinds);
    }//Awake

    /*
    * On Enable
    * Turns listener on when script is enabled
    */
    private void OnEnable()
    {
        _controls.Enable();
    }//OnEnable

    /*
    * On Disable
    * Turns listener off when script is disabled
    */
    private void OnDisable()
    {
        _controls.Disable();
    }//OnDisable

    /*
     * Start
     * Initializing loadout
     */
    void Start()
    {
        for (int i = 0; i <= 7; i++)
        {
            _inputs[i] = _controls.Player.Movement;
        }

        _inputs[8] = _controls.Player.Standard;
        _inputs[9] = _controls.Player.Bramble;
        _inputs[10] = _controls.Player.Warp;
        _inputs[11] = _controls.Player.Airburst;
        _inputs[12] = _controls.Player.Standard;
        _inputs[13] = _controls.Player.Bramble;
        _inputs[14] = _controls.Player.Warp;
        _inputs[15] = _controls.Player.Airburst;

        _inputs[16] = _controls.Player.Jump;
        _inputs[17] = _controls.Player.Interact;
        _inputs[18] = _controls.Player.Cancel;
        _inputs[19] = _controls.Player.Fire;
        _inputs[20] = _controls.Player.Zoom;
        _inputs[21] = _controls.Player.Crouch;
        _inputs[22] = _controls.Player.Crouch;

        _bindings[0] = 1;
        _bindings[1] = 2;
        _bindings[2] = 3;
        _bindings[3] = 4;
        _bindings[4] = 6;
        _bindings[5] = 7;
        _bindings[6] = 8;
        _bindings[7] = 9;

        _bindings[8] = 0;
        _bindings[9] = 0;
        _bindings[10] = 0;
        _bindings[11] = 0;
        _bindings[12] = 1;
        _bindings[13] = 1;
        _bindings[14] = 1;
        _bindings[15] = 1;

        _bindings[16] = 0;
        _bindings[17] = 0;
        _bindings[18] = 0;
        _bindings[19] = 0;
        _bindings[20] = 0;
        _bindings[21] = 0;
        _bindings[22] = 1;

        //Disables the active listener in the Rebinding Menu
        if (SceneManager.GetActiveScene().name == "HowToPlay")
        {
            _controls.Disable();
        }
    }//Start

    /*
    * Action By Name
    * Returns the input action object referenced by name
    */
    public InputAction ActionByName(string name)
    {
        InputAction result = null;
        switch (name)
        {
            case "Jump":
                result = _controls.Player.Jump;
                break;
            case "Interact":
                result = _controls.Player.Interact;
                break;
            case "Standard":
                result = _controls.Player.Standard;
                break;
            case "Bramble":
                result = _controls.Player.Bramble;
                break;
            case "Warp":
                result = _controls.Player.Warp;
                break;
            case "Airburst":
                result = _controls.Player.Airburst;
                break;
            case "Cancel":
                result = _controls.Player.Cancel;
                break;
            case "Movement":
                result = _controls.Player.Movement;
                break;
            case "Look":
                result = _controls.Player.Look;
                break;
            case "Crouch":
                result = _controls.Player.Crouch;
                break;
            case "Fire":
                result = _controls.Player.Fire;
                break;
            case "Zoom":
                result = _controls.Player.Zoom;
                break;
            default:
                break;
        }
        return result;
    }//ActionByName

    /*
    * First Key
    * Returns the first key binding as a human readable string
    */
    public string FirstKey(string name)
    {
        InputAction target = ActionByName(name);
        string report = InputControlPath.ToHumanReadableString(
            target.bindings[0].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);
        return report;
    }//FirstKey

    /*
    * Second Key
    * Returns the second key binding as a human readable string
    * This is necessary for the four arrow types and the crouch action
    */
    public string SecondKey(string name)
    {
        InputAction target = ActionByName(name);
        string report = InputControlPath.ToHumanReadableString(
            target.bindings[1].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);
        return report;
    }//SecondKey

    /*
    * By Part
    * Returns composite bindings as human readable strings
    * [0] and [5] display "2D Axis" as that is the type of control
    * [1 through 4] and [6 through 9] return keys bound to the axes
    * In order of Up, Down, Left, Right
    * Used for Movement and Look actions
    * Movement covers both sets, Look only deals with keys [1 through 4] 
    */
    public string ByPart(string name, int number)
    {
        InputAction target = ActionByName(name);
        string report = InputControlPath.ToHumanReadableString(
            target.bindings[number].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);
        return report;
    }//ByPart

    /*
    * Bindings Array Location
    * Returns Input Action stored at that index
    */
    public InputAction ActionsArrayLocation(int index)
    {
        return _inputs[index];
    }//BindingsArrayLocation

    /*
    * Bindings Array Location
    * Returns value of Input Binding stored at that index
    */
    public int BindingsArrayLocation(int index)
    {
        return _bindings[index];
    }//BindingsArrayLocation

    /*
    * Save Keys
    * Encodes binding overrides ands saves to PlayerPrefs
    */
    public void SaveKeys()
    {
        //From DapperDino's Tutorial
        string rebinds = _controls.SaveBindingOverridesAsJson();

        PlayerPrefs.SetString(_rebindsKey, rebinds);
    }//SaveKeys
}//DisplayKeys