using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    TMPro.TMP_Text txtScore;

    [SerializeField]
    UnityEngine.UI.Image HealthImage;
    [SerializeField]
    Sprite noHealth;
    [SerializeField]
    Sprite lowHealth;
    [SerializeField]
    Sprite halfHealth;
    [SerializeField]
    Sprite highHealth;
    [SerializeField]
    Sprite fullHealth;

    public int Points=0;
    public int Health = 100;

      // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        txtScore.text = "Score: " + Points.ToString();
        Debug.Log("Points" + Points.ToString() + "Health" + Health.ToString());

        //Update health img based on game health
        if (Health >= 100)
            HealthImage.sprite = fullHealth;
        else if (Health >= 75)
            HealthImage.sprite = highHealth;
        else if (Health >= 50)
            HealthImage.sprite = halfHealth;
        else if (Health >= 25)
            HealthImage.sprite = lowHealth;
        else
            HealthImage.sprite = noHealth;
    }
}
