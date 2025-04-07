using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveController : MonoBehaviour
{
    CharacterController playerController;

    Vector3 direction;

    [Header("玩家数值类")]
    [SerializeField]public float speed = 1;
    [SerializeField]public float jumpPower = 5;
    [SerializeField]public float gravity = 7f;
    [Header("视角数值类")]
    [SerializeField]public float mousespeed = 5f;
    [SerializeField]public float minmouseY = -45f;
    [SerializeField]public float maxmouseY = 45f;

    float RotationY = 0f;
    float RotationX = 0f;
    [SerializeField]private float localspeed = 0;
    [Header("储存类")]
    [SerializeField]public Transform agretctCamera;


    // Use this for initialization
    void Start()
    {
        localspeed = speed;
        playerController = this.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        SpeedUp();
        float _horizontal = Input.GetAxis("Horizontal");
        float _vertical = Input.GetAxis("Vertical");

        if (playerController.isGrounded)
        {
            direction = new Vector3(_horizontal, 0, _vertical);
            if (Input.GetKeyDown(KeyCode.Space))
                direction.y = jumpPower;
        }
        direction.y -= gravity * Time.deltaTime;
        playerController.Move(playerController.transform.TransformDirection(direction * Time.deltaTime * localspeed));

        RotationX += agretctCamera.transform.localEulerAngles.y + Input.GetAxis("Mouse X") * mousespeed;

        RotationY -= Input.GetAxis("Mouse Y") * mousespeed;
        RotationY = Mathf.Clamp(RotationY, minmouseY, maxmouseY);

        this.transform.eulerAngles = new Vector3(0, RotationX, 0);

        agretctCamera.transform.eulerAngles = new Vector3(RotationY, RotationX, 0);

    }

    public void SpeedUp()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            localspeed = speed*1.6f;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            localspeed = speed;
        }
    }
}
