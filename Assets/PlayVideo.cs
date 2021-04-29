using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayVideo : MonoBehaviour
{

 
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
     
       

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CloseVideo()
    {
        Debug.Log("Deactivating");

        /*
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        gameObject.SetActive(false);
        Debug.Log("Deactivating");
        */
        
    }
}
