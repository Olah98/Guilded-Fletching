/*
Author: Christian Mullins
Date: 02/15/2021
Summary: Class that interacts with and triggers the Door script class.
*/
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public enum SwitchType {
    Ladder, HingedObject, Platform
}

[Serializable]
public class Switch : MonoBehaviour {
    [HideInInspector] public List<Door> doorObjs;
    [HideInInspector] public List<HingedObject> hingedObjs;
    [HideInInspector] public List<CountingPlatform> platformObjs;

    [HideInInspector] public SwitchType mySwitchType;

    public bool isTimedByArrow;
    public bool isFlipped = false;
    [Tooltip("Check if the player is able to interact with this, or it can" +
            " only be triggered by arrows.")]
    public bool isInteractable;
    public bool triggerByArrow;

    //By Warren
    //Edit to change whether color or material is being affected.
    private Color _isOn = Color.green;
    private Color _isOff = Color.red;
    private List<MeshRenderer> _rend = new List<MeshRenderer>();

    private void Start()
    {
        //By Warren
        if (GetComponent<MeshRenderer>() != null)
            _rend.Add(GetComponent<MeshRenderer>());
        else if (GetComponentsInChildren<MeshRenderer>() != null)
            _rend = new List<MeshRenderer>(GetComponentsInChildren<MeshRenderer>());
        UpdateColor();

        // push this Switch into the Switch list of the object we're switching
        // if it exists
        doorObjs.ForEach(    obj => { obj.mySwitches.Add(this); });
        hingedObjs.ForEach(  obj => { obj.mySwitches.Add(this); });
        platformObjs.ForEach(obj => { obj.mySwitches.Add(this); });

        if (!isFlipped) {
            doorObjs.ForEach(    obj => { obj.UpdateColor(false); });
            hingedObjs.ForEach(  obj => { obj.UpdateColor(false); });
            platformObjs.ForEach(obj => { obj.UpdateColor(false); });
        }
    }

    /// <summary>
    /// Function to call when switch is hit by an arrow or interacting with.
    /// </summary>
    public virtual void HitSwitch()
    {
        if (isFlipped) return;

        isFlipped = true;
        
        doorObjs.ForEach(obj => {
            if (obj is TimedDoor) {
                var tDoor = (TimedDoor)obj;
                if (!tDoor.isClosing && tDoor.IsAllSwitchesFlipped())
                    StartCoroutine(tDoor.Open());
                else if (tDoor.isClosing)
                    isFlipped = false;
            }
            else if (obj.IsAllSwitchesFlipped())
                StartCoroutine(obj.Open());
        });
        hingedObjs.ForEach(obj => {
            if (obj.IsAllSwitchesFlipped())
                StartCoroutine(obj.Open());
        });
        platformObjs.ForEach(obj => { obj.IsAllSwitchesFlipped(); });
        // update color at the end in case the ForEach check reverses the flip
        UpdateColor();//By Warren
    }

    /// <summary>
    /// Function to call when switch is reset, such as by a timer running out.
    /// By Warren
    /// </summary>
    public void ResetSwitch()
    {
        if (!isFlipped) return;
        isFlipped = false;
        UpdateColor();
        if (this.tag == "Untagged")
        {
            this.tag = "Interactable"; //By Warren
        }
    }
/*
    //COMMENTED OUT BECAUSE THIS COLLISION IS NOW BEING HANDLED BY RAYCASTING INSTEAD
    /// <summary>
    /// Check if an arrow hits the switch, then activate.
    /// </summary>
    /// <param name="other">Object hitting the switch.</param>
    /// 
    protected void OnCollisionEnter(Collision other)
    {
        if (other.transform.tag == "Arrow" && triggerByArrow)
        {
            HitSwitch();
            if (isTimedByArrow)
            {
                Invoke("ResetSwitch", other.gameObject.GetComponent<Arrow>().stickTime);
            }
        }
    }
 */   
    public void ArrowHit(GameObject other)
    {
        HitSwitch();
        if (isTimedByArrow)
        {
            Invoke("ResetSwitch", other.GetComponent<Arrow>().stickTime);
        }
    }

    /// <summary>
    /// Function to call when changing color or material.
    /// Edit to change whether color or material is being affected.
    /// By Warren
    /// </summary>
    protected void UpdateColor()
    {
        Color change;
        if (isFlipped)
        {
            change = _isOn;
        }
        else
        {
            change = _isOff;
        }
        foreach (var r in _rend)
            r.material.SetColor("_Color", change);
    }
}