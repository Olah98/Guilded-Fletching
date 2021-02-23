using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ThankYou : MonoBehaviour
{
    public Text thankText;

    public string thank;

    

    private void Start()
    {
        thankText.text = " ";

        
    }

    public void interact()
    {
        thankText.text = "Thank you for testing please press Escape to pause and go to the main menu";
    }

    
}
