using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_FPS : MonoBehaviour
{
    public float MoveSpeed = 10;
    public float MouseSensitivity = 1;
    public Transform FPSCamera;
    private float RotateY = 0;
    private CharacterController thisController = null;

    public Transform Gun;
    public GameObject Bullet;
    public float ShootInterval = 0.1f;
    private float NextShoot = 0;

    public Transform CrossHairObject;

    private Vector3 playerVelocity;
    public float JumpHeight = 1;
    private float gravity = -20;

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
            MouseLook();
            Movement();
            Shoot();
        }
    }

    private void MouseLook()
    {
        float rotateX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * MouseSensitivity;

        RotateY -= Input.GetAxis("Mouse Y") * MouseSensitivity;
        RotateY = Mathf.Clamp(RotateY, -60, 60);
        FPSCamera.localEulerAngles = new Vector3( RotateY,  0, 0);
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, rotateX, transform.localEulerAngles.z);
    }

    private void Movement()
    {


        bool OnGround = thisController.isGrounded;

        if (OnGround && playerVelocity.y < 0)
            playerVelocity.y = 0;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 direction = (transform.forward * v + transform.right * h);
        thisController.Move(direction * Time.deltaTime * MoveSpeed);

        if (Input.GetKeyDown(KeyCode.Space) && OnGround)
            playerVelocity.y += Mathf.Sqrt(JumpHeight * -3.0f * gravity);

        playerVelocity.y += gravity * Time.deltaTime;
        thisController.Move(playerVelocity * Time.deltaTime);
    }

    private void Shoot()
    {
        if(Input.GetMouseButton(0) && Time.time >= NextShoot)
        {
            NextShoot = Time.time + ShootInterval;
            Instantiate(Bullet, Gun.transform.position + Gun.transform.forward, FPSCamera.transform.rotation);
        }
    }
}
