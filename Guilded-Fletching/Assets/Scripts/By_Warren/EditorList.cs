/*
Author: Warren Rose II
Data: 02/22/2021
Summary: Provides the Level Designers with a custom editor to build puzzles
Following Tutorial by Cat Like Coding:
* https://catlikecoding.com/unity/tutorials/editor/custom-list/
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
#if UNITY_EDITOR
public class EditorList : MonoBehaviour
{
    public static void Show(SerializedProperty list)
    {
    }
}
#endif
