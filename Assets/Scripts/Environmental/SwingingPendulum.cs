using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingingPendulum : MonoBehaviour, IBrambleable
{
    // Start is called before the first frame update
    [Tooltip("Default value is 1")]
    public float speed;
    public float startDelay;
    public int damage;
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
            collision.gameObject.GetComponent<Character>().TakeDamage(damage);
        }

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="enabled"></param>
    public void Bramble(in bool enabled) {
        anim.enabled = !enabled;
        force = (enabled) ? 0f : forceHolder;
    }

    public void Bind(float bindtime)
    {
        StartCoroutine(PauseAnimation(bindtime));
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
