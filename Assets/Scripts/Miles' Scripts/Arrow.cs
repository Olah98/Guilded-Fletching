using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Rigidbody rb;
    private bool stuck = false;
    // Start is called before the first frame update

    void Start()
    {
      
        rb = gameObject.GetComponent<Rigidbody>();
        transform.rotation = Quaternion.LookRotation(rb.velocity);
    }

    // Update is called once per frame
    void Update()
    {
        if (!stuck)
        {
            transform.rotation = Quaternion.LookRotation(rb.velocity);
            //Points rotation towards where it is headed.
           
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Stuck(collision.gameObject);
    }
    
    
    public void Stuck(GameObject other)
    {
        rb.constraints = RigidbodyConstraints.FreezeAll;
        gameObject.transform.parent = other.transform;

        //freezes object
    }


}
