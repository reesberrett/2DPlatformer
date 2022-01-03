using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnCollision : MonoBehaviour
{

    public string[] tagsOfObjectsToDestroyWhenHit;
    public GameObject prefabForDestoyEffects;
    public float secondsToPlayAnimation = 1;
    public string[] tagsOfObjectsToNotDestroyWhenHit;
    public bool destroyMeOnAnyCollision = true;
    public bool destroyMeOnTargetCollision = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        bool destroy = shouldDestroy(collision.gameObject.tag);
        if (prefabForDestoyEffects != null && destroy)
        {
            GameObject effect = Instantiate(prefabForDestoyEffects, transform.position, transform.rotation);
            Destroy(effect, secondsToPlayAnimation);
        }
        if (destroy)
        {
            Destroy(collision.gameObject);
        }

        if (destroyMeOnAnyCollision || (destroy && destroyMeOnTargetCollision))
            Destroy(gameObject);

    }

    bool shouldDestroy(string tag)
    {
        if(!string.IsNullOrEmpty(tag))
        {
            foreach (string s in tagsOfObjectsToNotDestroyWhenHit)
            {
                if (s == tag)
                    return false;
            }
            foreach (string s in tagsOfObjectsToDestroyWhenHit)
            {
                if (s == tag)
                    return true;
            }

        }
       
        if (tagsOfObjectsToNotDestroyWhenHit.Length > 0)
        {
            return true;//they have decided there are some tags to not destroy so make everything destroyable.
        }

        if (tagsOfObjectsToDestroyWhenHit.Length > 0)
        {
            return false;//They have decided there are specific tags to destory so make everything not destroyable.
        }
        return false;

    }

}
