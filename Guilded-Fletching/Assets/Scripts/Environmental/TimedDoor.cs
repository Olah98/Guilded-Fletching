/*
Author: Christian Mullins and Warren Rose II
Date: 04/2/2021
Summary: Based on Door class, resets after an amount of time.
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TimedDoor : Door
{
    public bool isClosing => _isClosing;
    public float yieldSeconds;
    private Vector3 _startpos;
    private bool _isClosing;


    protected override void Start()
    {
        base.Start();
        _isClosing = false;
        _startpos = transform.position;
    }

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
    /// Coroutine to close the Door outside of using Update().
    /// </summary>
    public virtual IEnumerator Close()
    {
        //Debug.Log("Close Triggered");
        // detach _moveTo child
        yield return new WaitForSeconds(yieldSeconds);
        ResetAllSwitches();
        _moveTo.parent = transform.parent;
        //Debug.Log("Close Started");
        do
        {
            Vector3 moveTo = (_startpos - transform.position).normalized;
            transform.Translate(moveTo * openSpeed * Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
            _isClosing = true;
        } while (!_IsPositionApproximateTo(_startpos));
        // re-attach _moveTo child
        _isClosing = false;
        _moveTo.parent = transform;
        yield return null;
    }

    /// <summary>
    /// Coroutine to open the Door outside of using Update().
    /// Modified to activate Close coroutine
    /// </summary>
    public override IEnumerator Open()
    {
        if (!_isClosing) {
            yield return StartCoroutine(base.Open());
            StartCoroutine(Close());
        }
    }
}