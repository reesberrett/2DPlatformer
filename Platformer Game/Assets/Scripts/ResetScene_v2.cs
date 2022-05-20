/*Copyright Jeremy Blair 2021
License (Creative Commons Zero, CC0)
http://creativecommons.org/publicdomain/zero/1.0/

You may use these scripts in personal and commercial projects.
Credit would be nice but is not mandatory.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetScene_v2 : MonoBehaviour
{
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



    public string tagOfObjectToReloadScene = "Player";
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (this.enabled)
        {
            if (collision.gameObject.tag.ToLower() == tagOfObjectToReloadScene.ToLower())
            {
                controller.DecreaseHealth();
                controller.RespawnPlayer();
            }
        }

    }

    public void Reload()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
