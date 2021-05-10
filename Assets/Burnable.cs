using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burnable : MonoBehaviour
{
    // Start is called before the first frame update
    public ParticleSystem Fire;
    public GameObject Vine;
    public bool burned;
    LayerMask mask;

    private void Start()
    {
        mask = LayerMask.GetMask("Burnable");
    }

    public void Burn()
    {
        burned = true;
        Fire.Play();
        Destroy(Vine, 1f);
        Collider[] hits = new Collider[3];
        // max number of collisions = hits.Length
        
        hits = Physics.OverlapBox(transform.position, new Vector3 (.25f, .25f, .25f), Quaternion.identity, mask);

        foreach (Collider hit in hits)
        {
            Debug.Log(hit.transform.name);
            if (hit.gameObject.tag == "Burnable" && !hit.gameObject.GetComponent<Burnable>().burned)
            {
                StartCoroutine(BurnDelay(hit.gameObject));
            }
            else
            {
                StartCoroutine(DestroyDelay());
            }    
        }
        
    }

 

    IEnumerator BurnDelay(GameObject obj)
    {
        yield return new WaitForSeconds(.5f);
        obj.GetComponent<Burnable>().Burn();
        yield return new WaitForSeconds(.5f);
        Fire.Stop();
        Destroy(gameObject, 2f);

    }

    IEnumerator DestroyDelay()
    {
        yield return new WaitForSeconds(1f);
        Fire.Stop();
        Destroy(gameObject, 2f);
    }

}
