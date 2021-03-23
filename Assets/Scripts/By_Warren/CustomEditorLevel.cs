/*
Author: Warren Rose II
Data: 02 / 22 / 2021
Summary: Provides the Level Designers with a custom editor to build puzzles
Following Tutorial by Cat Like Coding:
* https://catlikecoding.com/unity/tutorials/editor/custom-list/
Following Tutorial by Unity Learn
* https://learn.unity.com/tutorial/editor-scripting
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
#if UNITY_EDITOR
[CustomEditor(typeof(LevelManager))]
public class CustomEditorLevel : Editor
{
	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("complete"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("loadout"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("dragPuzzleManagerHere"));
		serializedObject.ApplyModifiedProperties();

		LevelManager myScript = (LevelManager)target;
		if (GUILayout.Button("Creates a New Node from Above Object"))
		{
			myScript.AddPuzzleManager(myScript.dragPuzzleManagerHere.GetComponent<PuzzleManager>());
			myScript.dragPuzzleManagerHere = null;
		}

		EditorGUILayout.PropertyField(serializedObject.FindProperty("index"), true);
		//Not showing if individual pieces are complete
		//for (int i = 0; i < myScript.index.Count; i++)
		//{
		//	EditorGUILayout.PropertyField(serializedObject.FindProperty("index[" + i + "].complete"));
		//}
	}
}
#endif
