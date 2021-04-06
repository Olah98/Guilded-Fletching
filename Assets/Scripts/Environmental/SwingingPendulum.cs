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
    private Animator anim;

    public void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        anim.speed = speed;

        StartCoroutine(PauseAnimation());
    }

    IEnumerator PauseAnimation()
    {
        anim.enabled = false;
        yield return new WaitForSeconds(startDelay);

        anim.enabled = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Colliding!");
        var contact = collision.contacts[0];
        Vector3 direction = collision.transform.position - contact.point;
        if (collision.transform.tag == "Player")
        {
            collision.gameObject.GetComponent<Character>().AddImpact(direction, force);
        }

    }



}
