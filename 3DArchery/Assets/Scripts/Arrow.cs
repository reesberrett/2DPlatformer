using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour{

    GameController controller;

    // Start is called before the first frame update
    void Start()
    {
        controller = GameObject.FindObjectOfType<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collider collision)
    {
        if (collision.gameObject.tag == "ArrowPoints")
            controller.Points += 1;
        if (collision.gameObject.tag == "ChickenNugget")
            controller.Points += 100;
    }
}
