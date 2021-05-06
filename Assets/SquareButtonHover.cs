/*
Author: Warren Rose II
Data: 04/7/2021
Summary: Resizes button for Haley's art to show clearly.
Reference: https://answers.unity.com/questions/1199251/onmouseover-ui-button-c.html
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SquareButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 _base = new Vector3(1f, 1f, 1f);
    private Vector3 _large = new Vector3(1.4f, 1.4f, 1f);

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GetComponent<Button>().interactable == true)
        {
            GetComponent<RectTransform>().localScale = _large;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (GetComponent<Button>().interactable == true)
        {
            GetComponent<RectTransform>().localScale = _base;
        }
    }
}
