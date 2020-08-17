using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_FPS : MonoBehaviour
{

    public float MouseSensitivity = 1;
    public Transform FPSCamera;
    private float RotateY = 0;
    private CharacterController thisController = null;
    
    private Vector3 playerVelocity;
    public float JumpHeight = 1;
    private float gravity = -20;

    #region WallJump
    private bool WallJed = false;
    #endregion

    #region Slides
    private bool Sliding = false;
    private float SlidingTime;
    private float SlideRotate = 0;
    private bool SlideFinish = false;
    private float CurrentYHeight;
    #endregion

    #region ExternalStuff
    public Slider StaminaBar;
    #endregion

    #region Stats
    private float Stamina = 100;
    private float MoveSpeed = 10;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        thisController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.CurrentState == GameManager.GameState.GameInProgress)
        {
            SlidingForward();
            DashAndCrash();
            //AllUIBars();
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
        }
    }

    private void MouseLook()
    {

        //Lets the player move the camera with their mouse
        float rotateX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * MouseSensitivity;

        RotateY -= Input.GetAxis("Mouse Y") * MouseSensitivity;
        RotateY = Mathf.Clamp(RotateY, -60, 60);
        FPSCamera.localEulerAngles = new Vector3( RotateY,  0, 0);
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, rotateX, transform.localEulerAngles.z);
    }

    private void Movement()
    {
        //Only let the player move themself when they are not sliding, if they are sliding they can't change the slide direction
        //Apart from the direction they were originally going before they start sliding
        if(Sliding == false)
        {
            //If the player is touching a ground, they will be able to jump and/or move
            bool OnGround = thisController.isGrounded;

            if (OnGround && playerVelocity.y < 0)
            {
                playerVelocity.y = 0;
                playerVelocity.x = 0;
                playerVelocity.z = 0;
                WallJed = false;
            }


            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            Vector3 direction = (transform.forward * v + transform.right * h);
            thisController.Move(direction * Time.deltaTime * MoveSpeed);
            //Jump
            if (Input.GetKeyDown(KeyCode.Space) && OnGround)
                playerVelocity.y += Mathf.Sqrt(JumpHeight * -3.0f * gravity);

            playerVelocity.y += gravity * Time.deltaTime;
            thisController.Move(playerVelocity * Time.deltaTime);
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
            SlideRotate += 50 * Time.deltaTime;

            SlidingTime += 1 * Time.deltaTime;

            transform.rotation = Quaternion.Euler(SlideRotate, transform.rotation.x, transform.rotation.z);
            gameObject.transform.position += (-transform.up * 3) * Time.deltaTime;

            //Use the height we Got so that we can keep the players Y value the same as CurrentYHeight
            //which is it's original Y value so that it doesn't go down below it's original Y value and
            //glitch through the map
            if (gameObject.transform.position.y <= CurrentYHeight)
            {
                gameObject.transform.position = new Vector3(gameObject.transform.position.x , CurrentYHeight);
            }

        }

        //Slide Duration, a.k.a how long the character will slide for
        if (SlidingTime >= 3)
        {
            SlideRotate -= 100 * Time.deltaTime;
            SlidingTime = 0;
            Sliding = false;
            SlideFinish = true;
        }

        //On slide finish, stop sliding and if rotation is greater than 0 (From sliding rotation would be 90)
        //Set the rotation back to 0
        if(Sliding == false && SlideRotate >= 0 && SlideFinish == true)
        {
            transform.rotation = Quaternion.Euler(SlideRotate, transform.rotation.x, transform.rotation.z);
            SlideFinish = false;
        }
        //Don't let the rotation be greater than 90
        if(SlideRotate >= 90)
        {
            SlideRotate = 90;
        }

        //Don't let the rotation be lesser than 0
        else if(SlideRotate <= 0)
        {
            SlideRotate = 0;
        }

        
    }

    private void AllUIBars()
    {
        StaminaBar.maxValue = 100;
        StaminaBar.value = Stamina;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!thisController.isGrounded && hit.normal.y < 0.1f) 
        {
            if (Input.GetKeyDown(KeyCode.Space) && WallJed == false)
            {
                WallJed = true;
                Debug.DrawRay(hit.point, hit.normal, Color.green, 1);
                playerVelocity.y = 0;
                playerVelocity.y += Mathf.Sqrt(JumpHeight * -3.0f * gravity);
                playerVelocity.x += hit.normal.x;
                playerVelocity.z += hit.normal.z;
            }
        }
    }
}
