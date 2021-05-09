/*
Author: Christian Mullins
Date: 4/16/2021
Summary: Editor script for the HingedObject class.
*/
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(HingedObject))]
public class HingedObjectEditor : Editor {
    protected HingedObject targetObj;
    protected SerializedObject thisTarget;
    protected SerializedProperty pivotPosProp;
    protected SerializedProperty rotationDirProp;

    private Axis _pivotPosition;
    private Axis _desiredRotation;

    protected virtual void OnEnable() {
        targetObj = (HingedObject)target;
        thisTarget = new SerializedObject(target);
        pivotPosProp = thisTarget.FindProperty("pivotPosition");
        rotationDirProp = thisTarget.FindProperty("rotationAxis");
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        var hingedObj = (HingedObject) target;
        //var myProp = serializedObject.FindPro
        _pivotPosition = (Axis) EditorGUILayout.EnumPopup("Pivot Position", _pivotPosition);
        //targetObj.pivotPosition = (Axis)EditorGUILayout.EnumPopup("Pivot Position", targetObj.pivotPosition);
        // handle pivot
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Apply Pivot"))
            hingedObj.SnapPivotTo(_pivotPosition);
        if (GUILayout.Button("Reset Pivot to Center"))
            hingedObj.SnapPivotPos(hingedObj.transform.localPosition);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        // handle rotation
        //create header here for rotational values
        //targetObj.rotationAxis = (Axis)EditorGUILayout.EnumPopup("Current Rotation", targetObj.rotationAxis);
        _desiredRotation = (Axis) EditorGUILayout.EnumPopup("Current Rotation", _desiredRotation);

        if (GUILayout.Button("Apply Rotation")) {
            var myProp = serializedObject.FindProperty("rotationAxis");
            EditorGUILayout.PropertyField(myProp);
            hingedObj.rotationAxis = _desiredRotation;
        }

        // force scene to be dirty if changes have occured (otherwise changes don't save)
        if (GUI.changed || thisTarget.hasModifiedProperties) {
            EditorUtility.SetDirty(target);
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }
        serializedObject.ApplyModifiedProperties();
        thisTarget.ApplyModifiedProperties();
    }
}