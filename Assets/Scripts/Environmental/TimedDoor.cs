/*
Author: Christian Mullins and Warren Rose II
Date: 04/2/2021
Summary: Based on Door class, resets after an amount of time.
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TimedDoor : Door {
    public int YieldSeconds;
    private Vector3 _startpos;


    protected override void Start()
    {
        base.Start();
        _startpos = transform.position;
    }

    /// <summary>
    /// Iterate through every Switch to reset them.
    /// </summary>
    /// 
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
    /// Coroutine to close the Door outside of using Update().
    /// </summary>
    public virtual IEnumerator Close()
    {
        //Debug.Log("Close Triggered");
        // detach _moveTo child
        yield return new WaitForSeconds(YieldSeconds);
        ResetAllSwitches();
        _moveTo.parent = transform.parent;
        //Debug.Log("Close Started");
        do
        {
            Vector3 moveTo = (_startpos - transform.position).normalized;
            transform.Translate(moveTo * openSpeed * Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
        } while (!_IsPositionApproximateTo(_startpos));
        // re-attach _moveTo child
        _moveTo.parent = transform;
        yield return null;
    }

    /// <summary>
    /// Coroutine to open the Door outside of using Update().
    /// Modified to activate Close coroutine
    /// </summary>
    public override IEnumerator Open()
    {
        //Debug.Log("Open Start");
        yield return StartCoroutine(base.Open());
        //Debug.Log("Open Done");
        StartCoroutine(Close());
        yield return null;
    }
}