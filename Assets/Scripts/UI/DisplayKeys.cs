/*
Author: Warren Rose II
Data: 05/02/2021
Summary: Interprets the keys bound to various actions as strings.
Referencing: DapperDino's UI Tutorial https://youtu.be/dUCcZrPhwSo
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DisplayKeys : MonoBehaviour
{
    private Controls _controls;

    /*
    * Awake
    * Creates local copy of a Controls object
    */
    private void Awake()
    {
        _controls = new Controls();
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
}//DisplayKeys
