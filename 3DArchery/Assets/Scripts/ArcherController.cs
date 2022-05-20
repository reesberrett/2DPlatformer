using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class ArcherController : MonoBehaviour
{
    Animator anim;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponentInParent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {  
        //Walking

        if(Input.GetAxisRaw("Vertical")>0)
        {    
            anim.SetBool("IsWalking", true);

        }
        else if (Input.GetAxisRaw("Vertical") == 0)
        {   
            anim.SetBool("IsWalking", false);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            anim.SetBool("IsWalking", true);
        }

        if (Input.GetKeyUp(KeyCode.W))
        {
            anim.SetBool("IsWalking", false);
        }

        //Running

        if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.LeftShift))
        {
            anim.SetBool("IsWalking", false);
            anim.SetBool("IsRunning", true);
        }

        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift))
        {
            anim.SetBool("IsWalking", true);
            anim.SetBool("IsRunning", false);
        }

        //Neither

        if (!Input.GetKey(KeyCode.W))
        {
            anim.SetBool("IsWalking", false);
            anim.SetBool("IsRunning", false);
        }



        //Attacking

        if (Input.GetKeyDown(KeyCode.F))
        {
            anim.SetTrigger("Attack");
        }

        if (Input.GetKeyUp(KeyCode.F))
        {
            anim.SetTrigger("Idle");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        print("Hit trigger");

        if (other.tag == "Poison Mist")
        {
            anim.SetTrigger("Die");
            rb.constraints = RigidbodyConstraints.FreezeAll;
            //ThirdPersonCharacter controller = GetComponentInParent<ThirdPersonCharacter>();
            //controller.m_MovingTurnSpeed = 0;
            //controller.m_StationaryTurnSpeed = 0;
        }
        
        
    }

    public void FireArrow()
    {

    }
}
