/*
Author: Christian Mullins
Date: 05/07/2021
Summary: This script handles event throwing from the Animation to the 
    PlayerAnimationController for polished features.
*/
using System;
using UnityEngine;

public class AnimEventListener : MonoBehaviour {
    private PlayerAnimationController _animController;

    private void OnEnable() {
        _animController = GetComponentInParent<PlayerAnimationController>();
    }

    /// <summary>
    /// Use with AnimationEvent to signal certain points in the reload animation
    /// </summary>
    /// <param name="eventStr">String of Event</param>
    public void RaiseEvent(string eventStr) {
        SwapPhase phase;
        switch (eventStr) {
            case "Started": phase = SwapPhase.Started;
                break;
            case "HandsOffScreen": phase = SwapPhase.HandOffScreen;
                break;
            case "HandsBackOnScreen": phase = SwapPhase.HandBackOnScreen;
                break;
            case "Ended": phase = SwapPhase.Ended;
                break;
            default:
                throw new ArgumentException("'" + eventStr + "' is not a valid event");
        }
        _animController.SetSwapPhase(phase);
    }
}
