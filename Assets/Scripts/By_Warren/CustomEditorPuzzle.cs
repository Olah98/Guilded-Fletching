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

[CustomEditor(typeof(PuzzleManager))]
public class CustomEditorPuzzle : Editor
{
	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("optional"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("complete"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("maxFalseNodesAllowed"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("dragPuzzlePieceHere"));
		serializedObject.ApplyModifiedProperties();

		PuzzleManager myScript = (PuzzleManager)target;
		if (GUILayout.Button("Creates a New Node (True) from Above Object"))
		{
			myScript.AddNodeTrue(myScript.dragPuzzlePieceHere);
			myScript.dragPuzzlePieceHere = null;
		}

		if (GUILayout.Button("Creates a New Node (False) from Above Object"))
		{
			myScript.AddNodeFalse(myScript.dragPuzzlePieceHere);
			myScript.dragPuzzlePieceHere = null;
		}


		EditorGUILayout.PropertyField(serializedObject.FindProperty("index"), true);
	}
}
