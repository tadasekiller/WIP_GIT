using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum GameState { GameOver, GameInProgress, GamePaused };
    public static GameState CurrentState = GameState.GamePaused;

    public static GameManager thisManager;
    public Text Txt_Message;
    public Slider StaminaBar;

    void Start()
    {
        thisManager = this;
    }

    void Update()
    {
        if (CurrentState == GameState.GamePaused)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                CurrentState = GameState.GameInProgress;
                Txt_Message.gameObject.SetActive(false);
            }
        }
        if (CurrentState == GameState.GameInProgress)
        {
            StaminaBar.maxValue = 100;
            StaminaBar.value = Player_FPS.thisPlayer.Stamina;
        }

        //restart the current scene
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
