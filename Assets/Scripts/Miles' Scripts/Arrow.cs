using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Rigidbody rb;
    private bool stuck = false;
    // Start is called before the first frame update

    void Start()
    {  
        transform.rotation = Quaternion.LookRotation(rb.velocity);
        Destroy(gameObject, 30f);
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

    void OnCollisionEnter(Collision collision)
    {
        if (stuck==false)
        {
            //Debug.Log("Collision!");
            Stuck();
        }
    }
    
    
    public void Stuck()
    {
        Debug.Log(gameObject.transform.rotation);
        gameObject.GetComponent<BoxCollider>().enabled = false;
        Vector3 temp = transform.rotation.eulerAngles;
        stuck = true;
        //rb.constraints = RigidbodyConstraints.FreezeAll;
        rb.isKinematic = true;
        rb.velocity = new Vector3(0, 0, 0);
        transform.rotation = Quaternion.Euler(temp);
        Debug.Log(transform.rotation);
        //gameObject.transform.parent = other.transform;

        //freezes object
    }
}
