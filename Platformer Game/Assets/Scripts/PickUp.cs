using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public string TagOfObjectToPickUp = "";
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == TagOfObjectToPickUp)
        {
            collision.gameObject.transform.parent = transform;
            collision.gameObject.SetActive(false);
        }
    }

}
