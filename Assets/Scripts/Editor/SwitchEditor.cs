/*
Author: Christian Mullins
Date: 2/15/2021
Summary: Editor portion of the Switch script.
*/
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(Switch))]
public class SwitchEditor : Editor {
    // editor variables
    Switch thisTarget;
    SerializedProperty switchType;
    SerializedObject targetObj;
    int curListSize;
    // lists
    SerializedProperty[] listProps; // called index through SwitchType Enum

    private void OnEnable() {
        thisTarget = (Switch)target;
        targetObj = new SerializedObject(thisTarget);
        switchType = targetObj.FindProperty("mySwitchType");
        listProps = new SerializedProperty[] {
            targetObj.FindProperty("doorObjs"),
            targetObj.FindProperty("hingedObjs"),
            targetObj.FindProperty("platformObjs")
        };
    }

    public override void OnInspectorGUI()
    {
        targetObj.Update();
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        // create enum property
        EditorGUILayout.LabelField("Switch List Values");
        thisTarget.mySwitchType = (SwitchType)EditorGUILayout.EnumPopup("Type", thisTarget.mySwitchType);

        // set display to selected enum
        var selectedProp = listProps[switchType.enumValueIndex];
        curListSize = selectedProp.arraySize;
        for (int i = 0; i < curListSize; ++i) {
            EditorGUILayout.PropertyField(selectedProp.GetArrayElementAtIndex(i));
        }

        EditorGUILayout.LabelField("Edit List");
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add New Item") && curListSize < 10) {
            selectedProp.InsertArrayElementAtIndex(curListSize);
        }
        if (GUILayout.Button("Delete Last Item") && curListSize > 0) {
            selectedProp.DeleteArrayElementAtIndex(curListSize - 1);
        }
        EditorGUILayout.EndHorizontal();

        // force scene to be dirty if changes have occured (otherwise changes don't save)
        if (GUI.changed) {
            EditorUtility.SetDirty(target);
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }
        targetObj.ApplyModifiedProperties();
        //serializedObject.ApplyModifiedProperties();
    }
}