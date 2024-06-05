using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Movement_Controller : MonoBehaviour
{
    //private Player_Input playerInputAction;
    [SerializeField] private Player_Input_Controller playerInputController;
    private CharacterController characterController;
    [SerializeField] private Animator characterAnimator;
    private Transform characterTransform;

    // animator hash
    private int isWalkingHash;
    private int isRunningHash;

    private Vector2 currentMovementInput;
    private Vector3 currentMovement;
    private Vector3 currentRunMovement;
    private bool isMovementPressed;
    private bool isRunPressed;
    [SerializeField] private float walkSpeed = 1.0f;
    [SerializeField] private float runMultiplier = 2.0f;
    [SerializeField] private float rotationFactorPerFrame = 1.0f;
    [SerializeField] private float gravity = 9.8f;
    [SerializeField] private float terminalVelocity = 100.0f;

    public bool isGrounded;


    private void Awake() 
    {
        if (characterAnimator == null) characterAnimator = GetComponentInChildren<Animator>();
        characterTransform = characterAnimator.transform;
        characterController = GetComponent<CharacterController>();

        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
    }
    private void Start() 
    {
        playerInputController.playerInputAction.Character.Move.started += ReadMovementInput;
        playerInputController.playerInputAction.Character.Move.canceled += ReadMovementInput;
        playerInputController.playerInputAction.Character.Move.performed += ReadMovementInput;
        playerInputController.playerInputAction.Character.Run.started += ReadRunButton;
        playerInputController.playerInputAction.Character.Run.canceled += ReadRunButton;
    }

    private void ReadMovementInput(InputAction.CallbackContext context)
    {
        currentMovementInput = context.ReadValue<Vector2>();
        isMovementPressed = currentMovementInput.magnitude != 0.0f;

        currentMovement.x = currentMovementInput.x * walkSpeed;
        currentMovement.z = currentMovementInput.y * walkSpeed;
        currentRunMovement = new Vector3(currentMovement.x * runMultiplier, currentMovement.y, currentMovement.z * runMultiplier);
    }

    private void ReadRunButton(InputAction.CallbackContext context)
    {
        isRunPressed = context.ReadValueAsButton();
    }

    private void OnEnable() 
    {
        playerInputController.playerInputAction.Character.Enable();
    }

    private void OnDisable() 
    {
        playerInputController.playerInputAction.Character.Disable();
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateGravity();
        UpdateAnimation();
        UpdateRotation();

        if(!isRunPressed)
        {
            characterController.Move(currentMovement * Time.deltaTime);
        }
        else
        {
            characterController.Move(currentRunMovement * Time.deltaTime);
        }

        isGrounded = characterController.isGrounded;
    }

    private void UpdateAnimation()
    {
        bool isWalking = characterAnimator.GetBool(isWalkingHash);
        bool isRunning = characterAnimator.GetBool(isRunningHash);

        if (isMovementPressed && !isWalking)
        {
            characterAnimator.SetBool(isWalkingHash, true);
        }
        if (!isMovementPressed && isWalking)
        {
            characterAnimator.SetBool(isWalkingHash, false);
        }
        if (isMovementPressed && isRunPressed && !isRunning)
        {
            characterAnimator.SetBool(isRunningHash, true);
        }
        if (!(isMovementPressed && isRunPressed) && isRunning)
        {
            characterAnimator.SetBool(isRunningHash, false);
        }
    }

    private void UpdateRotation()
    {
        if (!isMovementPressed) return;

        Vector3 targetPosition;
        targetPosition = new Vector3(currentMovement.x, 0.0f, currentMovement.z);

        Quaternion currentRotation = characterTransform.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(targetPosition);
        characterTransform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
    }

    private void UpdateGravity()
    {
        if (isGrounded)
        {
            float groundedGravity = -1.0f;
            currentMovement.y = groundedGravity;
            currentRunMovement.y = groundedGravity;
        }
        else if (currentMovement.y > -terminalVelocity)
        {
            currentMovement.y -= gravity * Time.deltaTime;
            currentRunMovement.y -= gravity * Time.deltaTime;
        }
    }

}
