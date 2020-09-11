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
    public Text Timer;
    public Text WallJump;
    private Outline walljoutline;

    public float TimerStart;

    void Start()
    {
        thisManager = this;
        walljoutline = WallJump.GetComponent<Outline>();
    }

    void Update()
    {
        if (CurrentState == GameState.GamePaused)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                CurrentState = GameState.GameInProgress;
                Txt_Message.gameObject.SetActive(false);
                TimerStart = Time.time;
            }
        }
        if (CurrentState == GameState.GameInProgress)
        {
            StaminaBar.maxValue = 100;
            StaminaBar.value = Player_FPS.thisPlayer.Stamina;
            Timer.text = "Time: " +(Time.time - TimerStart).ToString("F2");
            walljoutline.enabled = !Player_FPS.thisPlayer.WallJed;
        }

        //restart the current scene
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
