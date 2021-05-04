/*
Author: Christian Mullins
Date: 4/27/21
Summary: Editor changes so that the two animators in this script can be 
    copied to eachother and thus expediting troubleshooting of the animators.
*/
using UnityEditor;
using UnityEngine.Animations;
using UnityEditor.Animations;
using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

[Obsolete]
[CustomEditor(typeof(PlayerAnimationController))]
public class PlayerAnimationControllerEditor : Editor {
    //consts
    //const string animatorFilePath = "Assets/Animations/Animators/";
    //const string bowAnimName = "PlayerBow";
    //const string handAnimName = "PlayerHands";
    //class vars
    PlayerAnimationController myPAnim;
    SerializedObject thisTarget;
    //DateTime lastCopyUpdate = DateTime.time

    void OnEnable() {
        thisTarget = new SerializedObject(target);
        myPAnim = (PlayerAnimationController)target;
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        /*
        //output last time this button was used to copy?
        //use DateTime.ToFileTime?
        if (!AreAnimatorControllersSimilar(myPAnim.handAnimator, myPAnim.bowAnimator)) {
            var oldHandController = myPAnim.handAnimator.runtimeAnimatorController as AnimatorController;
            string lastWriteStr = FormatStr(File.GetLastWriteTime(AssetDatabase.GetAssetPath(oldHandController)));
            if (GUILayout.Button("Copy Bow AnimatorController to Hand AnimatorController,\n" + lastWriteStr)) {
                var newHandAnim = CreateNewHandAnimatorController();
                //find this asset path
                //get datetime for this
            }
        }
        else {
            //output some sort of feedback to tell they are already equal
        }
        */
        thisTarget.ApplyModifiedProperties();
    }

    AnimatorController CreateNewHandAnimatorController() {
        var bowToHandController = myPAnim.bowAnimator.runtimeAnimatorController as AnimatorController;
        var allStates = bowToHandController.layers[0].stateMachine.states;
        foreach (var s in allStates) {
            bowToHandController.SetStateEffectiveMotion(s.state, s.state.motion);
        }
        
        return bowToHandController;
    }


    bool AreAnimatorControllersSimilar(in Animator anim1, in Animator anim2) {
        var anim1Controller = anim1.runtimeAnimatorController as AnimatorController;
        var anim2Controller = anim2.runtimeAnimatorController as AnimatorController;
        var allAnim1States = anim1Controller.layers[0].stateMachine.states;
        var allAnim2States = anim2Controller.layers[0].stateMachine.states;
        var motionPlaceHolder = allAnim1States[0].state.motion;
        for (int i = 0; i < allAnim1States.Length; ++i) {
            anim1Controller.SetStateEffectiveMotion(new AnimatorState(), motionPlaceHolder);
            anim2Controller.SetStateEffectiveMotion(new AnimatorState(), motionPlaceHolder);
        }
        //Debug.Log("evaluates as: " + (anim1Controller == anim2Controller));
        return anim1Controller == anim2Controller;
    }

    string FormatStr(DateTime dateTime) {
        TimeSpan sinceLastSave = DateTime.Now - dateTime;
        string newFormatStr = string.Empty;
        if (sinceLastSave.Days > 0) 
            newFormatStr = sinceLastSave.Days + " day(s)";
        else if (sinceLastSave.Hours > 0)
            newFormatStr = sinceLastSave.Hours + " hour(s)";
        else
            newFormatStr = sinceLastSave.Minutes + " minute(s)";

        return newFormatStr + " since last copy update";
    }
}