using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class FPS_Controller : MonoBehaviour
{

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintSpeed = 6.0f;
    [SerializeField] private float crouchSpeed = 1.5f;
    [SerializeField] private float speed;

    [Header("Look Settings")]
    [SerializeField] private float mouseSensitivity = 2.0f;
    [SerializeField] private float upDownLimit = 65f;

    [Header("Jump Settings")]
    [SerializeField] private float distance = 1.0f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float gravity = -9.81f;

    private float verticalRotation;
    private Vector3 currentMovement = Vector3.zero;
    private CharacterController characterController;
    private Camera playerCamera;
    private bool canUncrouch;
    private bool isGrounded;
    private Vector3 velocity;
    private bool crouching;
    private bool sprinting;

    
    public Vector3 prevPos;
    public Vector3 currVel;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        characterController = this.gameObject.GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleLook();
        CheckCeiling();
        CheckGround();
    }

    void HandleMovement()
    {
        Vector3 horizontalMovement = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        if (Input.GetButton("Crouch"))
        {
            characterController.height = 0.9f;
            speed = crouchSpeed;
            crouching = true;
        }
        else if (canUncrouch == false)
        {
            characterController.height = 0.9f;
            speed = crouchSpeed;
            crouching = true;
        }
        else if (canUncrouch == true)
        {
            characterController.height = 1.8f;
            speed = walkSpeed;
            crouching = false;
        }

        if (Input.GetButton("Sprint") && isGrounded && !crouching)
        {
            Debug.Log("sprint");
            speed = sprintSpeed;
            sprinting = true;
        }
        else if (!sprinting && !crouching)
        {
            speed = walkSpeed;
        }

        horizontalMovement = transform.rotation * horizontalMovement;
        if (horizontalMovement.sqrMagnitude > 1f)
        {
            horizontalMovement.Normalize();
        }

        //characterController.Move(velocity * Time.deltaTime);

        currentMovement.x = horizontalMovement.x * speed;
        currentMovement.z = horizontalMovement.z * speed;

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

        if (velocity.y < 0f && isGrounded)
        {
            velocity.y = -2.0f;
        }

        characterController.Move(velocity * Time.deltaTime);
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(currentMovement * Time.deltaTime);

    }

    void HandleLook()
    {
        float horizontalRotation = Input.GetAxis("Mouse X") * mouseSensitivity;
        this.transform.Rotate(0f, horizontalRotation, 0f);

        verticalRotation -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -upDownLimit, upDownLimit);

        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }


    private void CheckGround()
    {
        Debug.DrawRay(transform.position, Vector3.down * distance, Color.red);

        if (Physics.Raycast((transform.position), Vector3.down * distance, out RaycastHit hit, distance))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void CheckCeiling()
    {
        Debug.DrawRay(transform.position, Vector3.up * distance, Color.red);

        if (Physics.Raycast((transform.position), Vector3.up * distance, out RaycastHit hit, distance))
        {
            canUncrouch = false;
        }
        else
        {
            canUncrouch = true;
        }
    }
}
