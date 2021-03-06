﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class Player_FPS : MonoBehaviour
{
    public static int Lives = 3;
    public static Player_FPS thisPlayer;
    public float MouseSensitivity = 1;
    public Transform FPSCamera;
    public GameObject CameraCameraCamera;
    private float RotateY = 0;
    private CharacterController thisController = null;
    private Transform spawnpoint;
    private Vector3 playerVelocity;
    public float JumpHeight = 1;
    private float gravity = -20;

    #region WallJump
    public bool WallJed = false;
    #endregion

    #region Slides
    private bool Sliding = false;
    private float SlidingTime;
    private float SlideRotate = 0;
    private bool SlideFinish = false;
    private float CurrentYHeight;
    #endregion

    #region ExternalStuff
    //public Slider StaminaBar;
    public Text PlayerLives;
    #endregion

    #region Stats
    public float Stamina = 100;
    private float MoveSpeed = 10;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        thisController = GetComponent<CharacterController>();
        thisPlayer = this;
        spawnpoint = GameObject.FindGameObjectWithTag("Spawn").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.CurrentState == GameManager.GameState.GameInProgress)
        {
            SlidingForward();
            DashAndCrash();
            //AllUIBars(); I moved the UI to the GameManager Script so we don't have to assign it for every scene
            MouseLook();
            Movement();
            #region Debug
            /*
            if (thisController.isGrounded)
            {
                Debug.Log("grounded");
            }
            else
            {
                Debug.Log("not");
            }
            */
            #endregion

            if(Sliding == false)
            {
                thisController.height = 2;
            }

            else if(Sliding == true)
            {
                thisController.height = 0;
            }

            PlayerLives.text = "Lives: " + Lives;
        }
    }

    private void MouseLook()
    {
        if(Sliding == false)
        {
            //Lets the player move the camera with their mouse
            float rotateX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * MouseSensitivity;

            RotateY -= Input.GetAxis("Mouse Y") * MouseSensitivity;
            RotateY = Mathf.Clamp(RotateY, -60, 60);
            FPSCamera.localEulerAngles = new Vector3(RotateY, 0, 0);
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, rotateX, transform.localEulerAngles.z);
        }
    }

    private void Movement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        //Only let the player move themself when they are not sliding, if they are sliding they can't change the slide direction
        //Apart from the direction they were originally going before they start sliding
        if (Sliding == false)
        {
            //If the player is touching a ground, they will be able to jump and/or move
            bool OnGround = thisController.isGrounded;

            if (OnGround && playerVelocity.y < 0)
            {
                //Reset Velocity and Gravity
                playerVelocity.y = 0;
                playerVelocity.x = 0;
                playerVelocity.z = 0;
                //Reset Walljump
                WallJed = false;
                //Reset Slope(Climb)
                thisController.slopeLimit = 45;
            }

            Vector3 direction = (transform.forward * v + transform.right * h);
            thisController.Move(direction * Time.deltaTime * MoveSpeed);
            //Jump
            if (Input.GetKeyDown(KeyCode.Space) && OnGround)
            {
                playerVelocity.y += Mathf.Sqrt(JumpHeight * -3.0f * gravity);
                thisController.slopeLimit = 90;
            }

            playerVelocity.y += gravity * Time.deltaTime;
            thisController.Move(playerVelocity * Time.deltaTime);
        }

        if(Sliding == true)
        {
            Vector3 direction = (-transform.up * v + -transform.right * h);
            thisController.Move(direction * Time.deltaTime * MoveSpeed);
        }
    }

    private void DashAndCrash()
    {
        if(Input.GetKey(KeyCode.LeftShift))
        {
            //On left shift, if the player has stamina then they can dash but deplete their stamina
            if (Stamina >= 30)
            {
                MoveSpeed = 20;
                Stamina -= 10 * Time.deltaTime;
                
            }

            else if (Stamina < 30 && Stamina > 0)
            {
                MoveSpeed = 5;
                Stamina -= 10 * Time.deltaTime;
            }

            else if(Stamina <= 0)
            {
                MoveSpeed = 0;
                Stamina = 0;
            }
        }

        else
        {
            MoveSpeed = 10;

        }

        //If players speed remsins normal and they will gain stamina up to their max stamina
        if (MoveSpeed == 10)
        {
            Stamina += 5 * Time.deltaTime;

            if (Stamina >= 100)
            {
                Stamina = 100;
            }
        }
    }

    //Sliding currently incomplete and Buggy
    private void SlidingForward()
    {
        //Set sliding to true when the player presses the left control key
        //get the current height to use for later codes
        if(Input.GetKeyDown(KeyCode.LeftControl) && Sliding == false)
        {
            CurrentYHeight = gameObject.transform.position.y;
            Sliding = true;
        }

        if(Sliding == true)
        {
            float rotateX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * MouseSensitivity;
            RotateY = Mathf.Clamp(RotateY, -100, 100);

            SlideRotate += 50 * Time.deltaTime;

            SlidingTime += 1 * Time.deltaTime;

            transform.rotation = Quaternion.Euler(-SlideRotate, transform.rotation.y, transform.rotation.z);
            FPSCamera.localEulerAngles = new Vector3(RotateY, 0, 0);
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, rotateX, transform.localEulerAngles.z);
            //Use the height we Got so that we can keep the players Y value the same as CurrentYHeight
            //which is it's original Y value so that it doesn't go down below it's original Y value and
            //glitch through the map
            if (gameObject.transform.position.y <= CurrentYHeight)
            {
                gameObject.transform.position = new Vector3(gameObject.transform.position.x , CurrentYHeight);
            }

            else if(gameObject.transform.position.y >= CurrentYHeight)
            {
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, CurrentYHeight);
            }

        }

        //Slide Duration, a.k.a how long the character will slide for
        if (SlidingTime >= 2)
        {
            SlidingTime = 0;
            Sliding = false;
            SlideFinish = true;
        }

        //On slide finish, stop sliding and if rotation is greater than 0 (From sliding rotation would be 90)
        //Set the rotation back to 0
        if(SlideFinish == true)
        {
            float rotateX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * MouseSensitivity;
            transform.rotation = Quaternion.Euler(0, rotateX, transform.rotation.z);
            SlideFinish = false;
            SlideRotate = 0;
        }
        //Don't let the rotation be greater than 90
        if(SlideRotate >= 85)
        {
            SlideRotate = 85;
        }

        //Don't let the rotation be lesser than 0

        
    }
    /* UI Code
    private void AllUIBars()
    {
        StaminaBar.maxValue = 100;
        StaminaBar.value = Stamina;
    }
    */
    //Walljump Method
    //Checks if colliding with Object
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //Checks if already jumping & If object is a wall
        if (!thisController.isGrounded && hit.normal.y < 0.1f) 
        {
            //Checks for Spacebar & If already walljumped before
            if (Input.GetKeyDown(KeyCode.Space) && WallJed == false)
            {
                //Sets walljumped to true
                WallJed = true;
                //Debug for collision
                Debug.DrawRay(hit.point, hit.normal, Color.green, 1);
                //resets velocity so player doesn't walljump inconsistently (maybe optional/bad?)
                playerVelocity.y = 0;
                //Jump
                playerVelocity.y += Mathf.Sqrt(JumpHeight * -3.0f * gravity);
                //Increase force in opposite direction
                //playerVelocity.x += hit.normal.x*(MoveSpeed*0.5f);
                //playerVelocity.z += hit.normal.z*(MoveSpeed*0.5f);
            }
        }
        else if (hit.transform.tag == "Death")
        {
            Lives -= 1;
            if(Lives <= 0)
            {
                SceneManager.LoadScene(4);
            }
            transform.position = spawnpoint.position;
            transform.rotation = spawnpoint.rotation;
            GameManager.thisManager.TimerStart = Time.time;
            Stamina = 100;
        }
    }
}
