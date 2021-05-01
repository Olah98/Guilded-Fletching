/*
 * 
 * Referencing: DapperDino's UI Tutorial https://youtu.be/dUCcZrPhwSo
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DisplayKeys : MonoBehaviour
{

    private Controls _controls;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Awake()
    {
        _controls = new Controls();
    }

    private void OnEnable()
    {
        _controls.Enable();
    }

    private void OnDisable()
    {
        _controls.Disable();
    }

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
    }



    public string ArraySize(string name)
    {
        switch (name)
        {
            case "Jump":
                break;
            case "Interact":
                break;
            case "Standard":
                break;
            case "Bramble":
                break;
            case "Warp":
                break;
            case "Airburst":
                break;
            case "Cancel":
                break;
            case "Movement":
                break;
            case "Look":
                break;
            case "Crouch":
                break;
            case "Fire":
                break;
            case "Zoom":
                break;
            default:
                break;
        }
        return "";
    }

    public string KeyNumber(string name, int position)
    {
        switch (name)
        {
            case "Jump":
                break;
            case "Interact":
                break;
            case "Standard":
                break;
            case "Bramble":
                break;
            case "Warp":
                break;
            case "Airburst":
                break;
            case "Cancel":
                break;
            case "Movement":
                break;
            case "Look":
                break;
            case "Crouch":
                break;
            case "Fire":
                break;
            case "Zoom":
                break;
            default:
                break;
        }
        return "" + position;
    }

    public string FirstKey(string name)
    {
        InputAction target = ActionByName(name);
        string report = InputControlPath.ToHumanReadableString(
            target.bindings[0].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);
        return report;
    }
}
