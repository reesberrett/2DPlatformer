/*Copyright Jeremy Blair 2021
License (Creative Commons Zero, CC0)
http://creativecommons.org/publicdomain/zero/1.0/

You may use these scripts in personal and commercial projects.
Credit would be nice but is not mandatory.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeLevel_v2 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string tagOfObjectToTriggerSceneLoad = "Player";
    public string sceneToLoad = "";
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if this script is disabled, return.
        if (!this.isActiveAndEnabled)
            return;

        if (collision.gameObject.tag.ToLower() == tagOfObjectToTriggerSceneLoad.ToLower())
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad);
    }


}
