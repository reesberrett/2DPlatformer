using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateOnCollisionWithObject : MonoBehaviour
{
    public string TagOfObjectBeingHeld = "";
    public string AnimationTriggerToFireWhenHit = "";
    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        foreach(Transform child in collision.gameObject.transform)
        {
            if(child.gameObject.tag == TagOfObjectBeingHeld)
            {
                anim.SetTrigger(AnimationTriggerToFireWhenHit);
                Destroy(child.gameObject);
            }
        }
    }
}
