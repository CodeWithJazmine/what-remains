using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    private GameManager gameManager;
    private Animator animator;

    [SerializeField] private ThirdPersonMovement movement;
    [SerializeField] private InteractionHandler interactionHandler;
    [SerializeField] private Survivor survivor;

    [Header("Input Actions")]
    private InputAction moveAction;
    private InputAction interactAction;
    private InputAction nextSurvivorAction;
    private InputAction crouchAction;
    private InputAction sprintAction;

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private float crouchedSpeed = 2f;
    [SerializeField] private bool isCrouched = false;
    private float speed;
    public bool isMovementEnabled = true;


    void Awake()
    {
        gameManager = GameManager.instance;
        if (gameManager == null || gameManager.playerInput == null)
        {
            //Debug.LogError("PlayerInputManager: GameManager or PlayerInput is missing");
            enabled = false;
            return;
        }

        animator = GetComponentInChildren<Animator>();
        //if (!animator) //Debug.LogWarning("PlayerInputManager: Can't get animator");

        if (TryGetComponent<Survivor>(out var s))
            survivor = s;

        // Input Actions
        moveAction = gameManager.playerInput.actions["Move"];
        interactAction = gameManager.playerInput.actions["Interact"];
        nextSurvivorAction = gameManager.playerInput.actions["NextSurvivor"];
        crouchAction = gameManager.playerInput.actions["Crouch"];
        sprintAction = gameManager.playerInput.actions["Sprint"];

        // Set default speed
        speed = walkSpeed;
    }

    void Update()
    {
        HandleMove();
        HandleInteraction();
        HandleNextSurvivor();
        HandleCrouch();
        HandleSprint();
    }

    void HandleMove()
    {
        if (isMovementEnabled)
        {

            Vector2 input = moveAction.ReadValue<Vector2>();
            if (input.sqrMagnitude > 0.01f)
                movement.Move(input, speed);

            UpdateAnimation(input);
        }
    }

    void HandleInteraction()
    {
        if (interactAction.WasPressedThisFrame())
            interactionHandler.TryInteract();
    }

    void HandleNextSurvivor()
    {
        if (nextSurvivorAction.WasPressedThisFrame())
            GameManager.instance.NextSurvivor();
    }

    void HandleCrouch()
    {
        if (sprintAction.IsPressed()) return;

        if (crouchAction.WasPressedThisFrame())
        {
            ToggleCrouch();
        }
    }

    bool ToggleCrouch()
    {
        isCrouched = !isCrouched;

        speed = isCrouched ? crouchedSpeed : walkSpeed;

        if (animator != null)
        {
            animator.SetBool("IsCrouched", isCrouched);
        }

        return isCrouched;
    }

    void HandleSprint()
    {
        bool sprintHeld = sprintAction.IsPressed();

        if (sprintHeld)
        {
            // Sprint forces standing
            if (isCrouched)
            {
                isCrouched = false;

                if (animator != null)
                    animator.SetBool("IsCrouched", false);
            }

            speed = runSpeed;

            if (animator != null)
                animator.SetBool("IsSprinting", true);
        }
        else
        {
            speed = isCrouched ? crouchedSpeed : walkSpeed;

            if (animator != null)
                animator.SetBool("IsSprinting", false);
        }
    }

    void UpdateAnimation(Vector2 input)
    {
        // Rotate the survivor in the moving direction
        if (survivor.visualRoot != null)
        {
            if (input.x < -0.01f)
                survivor.visualRoot.localRotation = Quaternion.Euler(0f, -90f, 0f);
            else if (input.x > 0.01f)
                survivor.visualRoot.localRotation = Quaternion.Euler(0f, 90f, 0f);
        }

        // Animate survivor
        if (animator != null)
        {
            float speedParam = input.magnitude;
            animator.SetFloat("Speed", speedParam);
        }
    }
}
