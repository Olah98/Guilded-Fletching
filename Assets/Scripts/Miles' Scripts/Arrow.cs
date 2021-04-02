using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Rigidbody rb;
    private bool stuck = false;
    public float stickTime = 30f;
    public bool isLit;
    // Start is called before the first frame update

    void Start()
    {
        transform.rotation = Quaternion.LookRotation(rb.velocity);
    }

    // Update is called once per frame
    void Update()
    {
        if (!stuck)
        {
            // the arrow yields better results using transform.right instead of
            transform.right = -rb.velocity;
            //transform.rotation = Quaternion.LookRotation(rb.velocity); //OG
            //Points rotation towards where it is headed.
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (stuck==false)
        {

            if (collision.gameObject.tag=="Burnable" && isLit)
            {
                Destroy(collision.gameObject);
                return;
            }
            Debug.Log("Collision!");
            Stuck();
        }
    }


    public void Stuck()
    {
        //Debug.Log(transform.rotation);
        GetComponent<BoxCollider>().enabled = false;
        Vector3 temp = transform.rotation.eulerAngles;
        stuck = true;
        //rb.constraints = RigidbodyConstraints.FreezeAll;

        rb.velocity = new Vector3(0, 0, 0);
        transform.rotation = Quaternion.Euler(temp);

        //gameObject.transform.parent = other.transform;

        //freezes object

        Destroy(gameObject, stickTime); //Moved By Warren
    }

    /// <summary>
    /// Only used for when the arrow hits fire.
    /// </summary>
    /// <param name="other">Checking for "Fire" tag.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Fire")
        {
            isLit = true;
        }
    }


}
