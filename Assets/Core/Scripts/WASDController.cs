using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WASDController : MonoBehaviour
{
    [SerializeField] CharacterController controller;
    [SerializeField] GameObject playerGO;
    [SerializeField] Camera playerCam;
    [SerializeField] Transform groundCollision;
    [SerializeField] float groundDistance;
    [SerializeField] LayerMask groundMask;
    bool isGrounded;
    bool hasJumped;
    float lastJump;
    float bufferedJump;
    [SerializeField] float bufferedJumpDuration;

    [SerializeField] float cameraNormalFOV;
    [SerializeField] float cameraSprintingFOV;

    [SerializeField] float normalMoveSpeed;
    [SerializeField] float sprintingMoveSpeed;
    [SerializeField] float timeToFullSpeed;
    private float currentSpeed;
    private float sprintingTime;

    [SerializeField] float playerMouseSensitivity;
    float currentMouseSensitivity;

    [SerializeField] float momentumLossSpeed;
    Vector3 momentum;

    [SerializeField] float gravity = -9.81f;
    [SerializeField] float jumpStrnength = 10f;
    private float yVelocity;


    private float xRotation;
    // Start is called before the first frame update
    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        currentMouseSensitivity = playerMouseSensitivity;
        currentSpeed = normalMoveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        CheckGravity();
        CheckWalk();
        UpdateCamera();
        CheckCursorLock();
        CheckJump();
        CheckSprint();
    }

    private void CheckSprint()
    {
        sprintingTime = Mathf.Clamp(sprintingTime, 0, timeToFullSpeed);
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            if (isGrounded)
            {
                sprintingTime += Time.deltaTime;
            }
            else
            {

            }
        }
        else
        {
            if (isGrounded)
            {
                sprintingTime -= Time.deltaTime * 2;
            }
            else
            {
                sprintingTime -= Time.deltaTime * .5f;
            }
        }
        playerCam.fieldOfView = Mathf.Lerp(cameraNormalFOV, cameraSprintingFOV, Mathf.Clamp01(sprintingTime / timeToFullSpeed));
        currentSpeed = Mathf.Lerp(normalMoveSpeed, sprintingMoveSpeed, Mathf.Clamp01(sprintingTime / timeToFullSpeed));
    }

    private void CheckJump()
    {
        lastJump += Time.deltaTime;
        bufferedJump -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            bufferedJump = bufferedJumpDuration;
            if (!hasJumped)
            {
                bufferedJump = 0;
                yVelocity += jumpStrnength;
                hasJumped = true;
                lastJump = 0;
                momentum *= 2;
            }
        }
        if (bufferedJump > 0)
        {
            if (!hasJumped)
            {
                bufferedJump = 0;
                yVelocity += jumpStrnength;
                hasJumped = true;
                lastJump = 0;
                momentum *= 2;
            }
        }
    }

    private void CheckGravity()
    {
        isGrounded = Physics.CheckSphere(groundCollision.position, groundDistance, groundMask);
        if (isGrounded && lastJump > 0.3f)
        {
            hasJumped = false;
        }
        if (isGrounded && yVelocity < -2f)
        {
            yVelocity = -2f;
        }
        yVelocity += gravity * Time.deltaTime;

        controller.Move(new Vector3(0, yVelocity * Time.deltaTime, 0));
    }

    private void CheckCursorLock()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt))
        {
            Cursor.lockState = CursorLockMode.None;
            currentMouseSensitivity = 0;
        }
        if (Input.GetKeyUp(KeyCode.LeftAlt) || Input.GetKeyUp(KeyCode.RightAlt))
        {
            Cursor.lockState = CursorLockMode.Locked;
            currentMouseSensitivity = playerMouseSensitivity;
        }
    }

    private void UpdateCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * currentMouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * currentMouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //playerCam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerGO.transform.Rotate(Vector3.up * mouseX);
    }

    void CheckWalk()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 movement = playerCam.transform.right * x + playerCam.transform.forward * z + playerCam.transform.up * z;
        Move(movement);
    }

    void Move(Vector3 movement)
    {
        controller.Move((movement + momentum) * currentSpeed * Time.deltaTime);
        UpdateMomentum(movement);
    }

    void UpdateMomentum(Vector3 movement)
    {
        momentum += movement * Time.deltaTime;
        momentum /= 1 + (momentumLossSpeed * Time.deltaTime);
    }
}
