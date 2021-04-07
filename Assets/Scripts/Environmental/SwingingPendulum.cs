using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingingPendulum : MonoBehaviour
{
    // Start is called before the first frame update
    [Tooltip("Default value is 1")]
    public float speed;
    public float startDelay;
    [Tooltip("Dictates how far a player or enemy is hit.")]
    public float force;
    private float forceHolder;
    private Animator anim;

    public void Start()
    {
        forceHolder = force;
        anim = gameObject.GetComponent<Animator>();
        anim.speed = speed;

        StartCoroutine(PauseAnimation(startDelay));
    }

    

    private void OnCollisionEnter(Collision collision)
    {
        
        var contact = collision.contacts[0];
        Vector3 direction = collision.transform.position - contact.point;
        if (collision.transform.tag == "Player")
        {
            collision.gameObject.GetComponent<Character>().AddImpact(direction, force);
        }

        if (collision.transform.tag== "Arrow")
        {
            Debug.Log(collision.transform.gameObject.name);

            if (collision.gameObject.GetComponent<BrambleArrow>())
            {
                StartCoroutine(PauseAnimation(collision.gameObject.GetComponent<BrambleArrow>().bindTime));
            }
        }

    }

    IEnumerator PauseAnimation(float pauseTime)
    {
        force = 0;
        anim.enabled = false;
        yield return new WaitForSeconds(pauseTime);
        force = forceHolder;
        anim.enabled = true;
    }



}
