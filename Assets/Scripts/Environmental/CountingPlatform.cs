/*
Author: Christian Mullins & Warren Rose II
Date: 03/23/2021
Summary: Based on combining Door and MovingPlatform. Platform halts after time.
*/

using UnityEngine;
using System.Collections.Generic;

public class CountingPlatform : MovingPlatform
{
    [HideInInspector]
    public List<Switch> mySwitches;
    [Header("Waypoints to travel between")]
    public int maxRounds;
    private int _curRounds;

    //Edit to change whether color or material is being affected.
    private Color _isOn = Color.green;
    private Color _isOff = Color.red;
    private MeshRenderer _rend;
    //private bool _unlockedPlatform = false;

    //protected void Awake()
    //{
    //    _curRounds = maxRounds;
    //}

    protected override void Start()
    {
        base.Start();

        //By Warren
        _rend = GetComponent<MeshRenderer>();
        UpdateColor(true);
        IsAllSwitchesFlipped();
    }

    protected override void FixedUpdate()
    {
        // don't execute unless all of the switches are flipped
        //if (!_unlockedPlatform) return;
        if (_curRounds > 0) //by Warren
            base.FixedUpdate();
    }

    /// <summary>
    /// Set private variable _lockPlatform to be true publically.
    /// </summary>
    //public void UnlockPlatform() {
    //    _unlockedPlatform = true;
    //}


    /// <summary>
    /// Iterate through every Switch to reset them.
    /// </summary>
    public void ResetAllSwitches()
    {
        foreach (var s in mySwitches)
        {
            if (s.isFlipped)
            {
                s.ResetSwitch();
            }
        }
        UpdateColor(false);
    }

    /// <summary>
    /// Function to call when changing color or material.
    /// Edit to change whether color or material is being affected.
    /// By Warren
    /// </summary>
    public void UpdateColor(bool isActive)
    {
        Color change;

        if (isActive)
        {
            change = _isOn;
        }
        else
        {
            change = _isOff;
        }
        if (_rend != null)
        {
            _rend.material.SetColor("_Color", change);
        }
    }


    /// <summary>
    /// Iterate through every Switch to check if they are all flipped.
    /// </summary>
    /// <returns>True if all switches are flipped.</returns>
    public bool IsAllSwitchesFlipped()
    {
        foreach (var s in mySwitches)
        {
            if (!s.isFlipped)
            {
                UpdateColor(false);
                return false;
            }
        }
        UpdateColor(true);
        if (_curRounds == 0)
        {
            _curRounds = maxRounds;
        }
        return true;
    }

    /// <summary>
    /// Waypoints are checked through trigger system.
    /// </summary>
    /// <param name="other">Colliding object.</param>
    protected override void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Waypoint" && other.transform == curNode.Value)
        {
            //Debug.Log(_curRounds);
            if (_curRounds > 0)
            {
                _curRounds--;

                if (_curRounds == 0)
                {
                    ResetAllSwitches();
                }
            }
        }
        base.OnTriggerEnter(other); //moved by Warren
    }
}
