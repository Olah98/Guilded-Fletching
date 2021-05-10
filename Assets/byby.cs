using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class byby : MonoBehaviour
{
    public float timetodie = 0;
    private void Start()
    {
        Destroy(this.gameObject, timetodie);
    }
    private void Update()
    {
        if(Input.GetKey("escape"))
        {
            Destroy(this.gameObject);
        }
    }




}
