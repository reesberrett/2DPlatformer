using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedFish : MonoBehaviour
{
    Animator anim;
    public float speed = 0.001f;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInParent<Animator>();
        anim.SetFloat("AnimationTime", Random.RandomRange(0.0f, 1.0f));
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("Speed", speed);
        transform.Translate(0, 0, speed/100f);
        transform.Rotate(0, 0.01f, 0);
    }
}
