using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Rigidbody rb;
    private bool stuck = false;
    public float stickTime = 30f;
    public bool isLit;
    LayerMask mask;
    public enum Type { standard, bramble, warp, airburst}
    public Type type;
    // Start is called before the first frame update

    void Start()
    {
        mask = LayerMask.GetMask("Character", "Arrow");
        transform.rotation = Quaternion.LookRotation(rb.velocity);
        Destroy(gameObject, stickTime);
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

    private void FixedUpdate()
    {
        if (!stuck)
        {
            HitDetection();
        }
        
    }



    public void Stuck()
    {
        stuck = true;
       
        GetComponent<BoxCollider>().enabled = false;
        rb.useGravity = false;
        
        rb.velocity = new Vector3(0, 0, 0);
        rb.isKinematic = true;
        rb.freezeRotation = true;
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

    public void HitDetection()
    {
        float raycastDistance;
        raycastDistance = (Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.y) + Mathf.Abs(rb.velocity.z))/46f;

        if (raycastDistance <.5f)
        {
            raycastDistance = .5f;
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.right * -1f, out hit, raycastDistance, ~mask))
        {
          
            Stuck();
            string name = transform.name;
            Debug.Log(name);
            transform.position = hit.point;

            //Burn objects and switch objects
            if (hit.transform.tag == "Burnable" && isLit)
            {
                Destroy(hit.transform.gameObject);
                return;
            }
            else if (hit.transform.gameObject.GetComponent<Switch>())
            {
                if (hit.transform.gameObject.GetComponent<Switch>().triggerByArrow)
                {
                    hit.transform.gameObject.GetComponent<Switch>().ArrowHit(gameObject);
                }
            }

            //Dictate arrow used
            switch (type)
            {
                case Type.airburst:
                    gameObject.GetComponent<AirburstArrow>().Use();
                    break;
                case Type.bramble:
                    gameObject.GetComponent<BrambleArrow>().Use(hit.transform.gameObject);
                    break;
                case Type.warp:
                    gameObject.GetComponent<WarpArrow>().Use(hit.transform.gameObject);
                    break;
                case Type.standard:
                    gameObject.GetComponent<BaseArrow>().Impact(hit.transform.gameObject);
                    break;
                default:
                    return;
            }
            
        }
    }


}
