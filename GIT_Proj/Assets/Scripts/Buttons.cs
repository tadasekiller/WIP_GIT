using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoToTitleScreen()
    {
        Player_FPS.Lives = 3;
        GameManager.CurrentState = GameManager.GameState.GamePaused;
        SceneManager.LoadScene(0);
    }

    public void StartGame()
    {
        Player_FPS.Lives = 3;
        GameManager.CurrentState = GameManager.GameState.GamePaused;
        SceneManager.LoadScene(1);
    }
}
