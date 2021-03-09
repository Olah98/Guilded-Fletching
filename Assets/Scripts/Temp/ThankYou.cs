using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ThankYou : MonoBehaviour
{
    public string level;

    public void load()
    {
        SceneManager.LoadScene(level);
    }


}
