using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    [SerializeField]
    GameObject SpawnPoint;

    [SerializeField]
    public int health = 3;
    
    [SerializeField]
    List<LifeDrop> hearts;

    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        //health = hearts.Count;
        //if (PlayerPrefs.HasKey("Health"))
        //{
        //    health = PlayerPrefs.GetInt("Health");

        //    if(health<=0)
        //    {
        //        health = 3;
        //        PlayerPrefs.SetInt("Health", health);
        //    }
        //LifeDrop[] lds = GameObject.FindObjectsOfType<LifeDrop>();
        //hearts = new List<LifeDrop>();
        //for(int i=0;i<lds.Length;i++)
        //{
        //    hearts.Add(lds[i]);
        //}

        //}
        UpdateHearts();
    }

    //private void Awake()
    //{
    //    GameController[] objs = GameObject.FindObjectsOfType<GameController>();

    //    if (objs.Length > 1)
    //    {
    //        Destroy(this.gameObject);
    //    }
    //    DontDestroyOnLoad(this.gameObject);
    //}
    // Update is called once per frame
    void Update()
    {
        
    }

    public void DecreaseHealth()
    {
        health -= 1;

        //PlayerPrefs.SetInt("Health",health);
        UpdateHearts();
        if (health == 0)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameOverScreen");
        }

    }

    private void UpdateHearts()
    {
        for (int i = hearts.Count - 1; i >= health; i--)
        {
            hearts[i].DisplayNoHeart();
        }
    }

    public void RespawnPlayer()
    {
        player.transform.position = SpawnPoint.transform.position;
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        rb.velocity = Vector3.zero;
    }

}
