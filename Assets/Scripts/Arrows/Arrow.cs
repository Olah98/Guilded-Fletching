using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    //Author Miles Gomez
    //Edited 4/13/2021
    [Header ("Rigidbody")]
    public Rigidbody rb;
    private bool stuck = false;
    [Header ("Time that the arrow is alive for")]
    [Tooltip ("Deletes after *stickTime* seconds")]
    public float stickTime = 30f;
    [Header ("Bool determining whether or not the arrow is on fire")]
    public bool isLit;
    [Header ("Particle system on the arrows, only standard should have one")]
    public GameObject ps;
    LayerMask mask;
    public enum Type { standard, bramble, warp, airburst}
    [Header("Determine the type of arrow being shot")]
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
            if (ps!=null)
            {
                ps.GetComponent<ParticleSystem>().Play();
                ps.tag = "Fire";
            }
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
            transform.position = hit.point;

            //Burn objects and switch objects
            if (hit.transform.tag == "Burnable" && isLit)
            {
                Destroy(hit.transform.gameObject);
                Destroy(gameObject);
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
