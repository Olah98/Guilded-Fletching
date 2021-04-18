/*
Author: Christian Mullins
Date: 4/17/2021
Summary: Inherittance from HingedObjectEditor
*/
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(SwingingObject))]
public class SwingingObjectEditor : HingedObjectEditor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        EditorGUILayout.PropertyField(thisTarget.FindProperty("stopTimer"));

        if (GUI.changed) {
            EditorUtility.SetDirty(target);
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }
        thisTarget.ApplyModifiedProperties();
    }
}
