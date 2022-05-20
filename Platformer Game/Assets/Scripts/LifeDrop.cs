using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeDrop : MonoBehaviour
{
    [SerializeField]
    public int Health = 3;
    [SerializeField]
    Sprite FullHeart;
    [SerializeField]
    Sprite NoHeart;
    UnityEngine.UI.Image HeartImage;


    private void Start()
    {
        HeartImage = GetComponent<UnityEngine.UI.Image>();
        HeartImage.sprite = FullHeart;
    }
    public void DisplayNoHeart()
    {
        HeartImage.sprite = NoHeart;
    }

}
