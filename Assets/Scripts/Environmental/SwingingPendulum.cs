using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingingPendulum : MonoBehaviour
{
    // Start is called before the first frame update
    public bool left;
    public bool right;
    public float rotationAmount;
    public GameObject leftCube;
    public GameObject rightCube;
    public GameObject centerCube;
    private float swingAmount;
    void Start()
    {

     

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0f, rotationAmount, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == centerCube)
        {
            swingAmount = swingAmount * -1;
        }
    }

}
