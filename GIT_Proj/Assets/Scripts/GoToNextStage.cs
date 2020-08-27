using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToNextStage : MonoBehaviour
{
    public GameObject GoalText;
    private GameObject player;
    public int NextScene;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        GoalText.transform.LookAt(player.transform);
        //GoalText.transform.rotation = new Quaternion(GoalText.transform.rotation.x, 0, GoalText.transform.rotation.z,GoalText.transform.rotation.w);
    }

    private void OnTriggerEnter(Collider Other)
    {
        if(Other.tag == "Player")
        {
            //Name of Stage 2
            SceneManager.LoadScene(NextScene);
            GameManager.CurrentState = GameManager.GameState.GamePaused;
        }
    }
}
