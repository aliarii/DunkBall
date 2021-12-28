using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverUI;
    public static bool isGameOver;
    // Start is called before the first frame update
    void Start()
    {
        isGameOver = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameOver == true)
        {
            gameOverUI.SetActive(true);
        }
        if (isGameOver == true && SwipeManager.tap)
        {
            SceneManager.LoadScene("SampleScene");
        }
    }
}
