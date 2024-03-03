using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Movement3D))]
[RequireComponent(typeof(PlayerInput))]
public class CharacterMovement : MonoBehaviour
{
    [Header("Player")]
    public float movementSpeed = 20f;
    public float dashSpeedCoef = 3.0f;


    public float jumpForse = 20f;
    public float littleJumpForse = 10f;

    public float jumpInMidAirMaxNumber = 1;

    public float diveForce = 100f;


    public float hangTime = 0.2f;
    public float jumpCoolDown = 0.1f;
    public float jumpBufferLength = 0.3f;

    public float dashCoolDown = 0.1f;
    public float dashDuration = 0.05f;
    public int dashInMidAirMaxNumber = 2;


    public PlayerHealth playerHealth;


    private Movement3D movement3D;

    private PlayerInputs inputActions;

    private GameObject _mainCamera;


    private Vector2 movement2D = Vector2.zero;

    private float jumpCounter = 0f;
    private int jumpInMidAirCounter = 0;

    private float dashCounter = 0f;
    private float dashDurationCounter = 0f;

    private int dashInMidAirCounter = 0;

    private bool isGrounded = false;
    private bool isSupposedToJump = false;
    private bool isSupposedToLittleJump = false;

    private bool isSupposedToDash = false;
    private bool isDashing = false;



    private bool isLocked = false;
    
    public void Lock()
    {
        isLocked = true;
        
        inputActions.Disable();
    }

    public void Unlock()
    {
        isLocked = false;
        
        inputActions.Enable();
    }




    private void Move2D(InputAction.CallbackContext context)
    {
        movement2D = context.ReadValue<Vector2>();
        if (movement2D.sqrMagnitude > 1.0)
        {
            movement2D.Normalize();
        }
    }


    private void Jump(InputAction.CallbackContext _)
    {
        // jumpBufferCounter = jumpBufferLength;
        isSupposedToJump = true;
    }

    private void SlowDownJump(InputAction.CallbackContext _)
    {
        if (isSupposedToJump)
        {
            isSupposedToLittleJump = true;
        }

        movement3D.SlowDownJump();
    }

    // private void StartJumpingDown(InputAction.CallbackContext _)
    // {
    //     isJumpingDown = true;
    // }

    // private void StopJumpingDown(InputAction.CallbackContext _)
    // {
    //     isSupposedToStopJumpingDown = true;
    // }

    private void Dive(InputAction.CallbackContext _)
    {
        movement3D.Dive(diveForce);

        //print("Dive");
    }

    private void SlowDownDive(InputAction.CallbackContext _)
    {
        //print("SlowDownDive");
    }

    private void Dash(InputAction.CallbackContext _)
    {
        // movement3D.Dash(dashForce);

        isSupposedToDash = true;

        //print("Dash");
    }

    private void SlowDownDash(InputAction.CallbackContext _)
    {
        //print("SlowDownDash");
    }

    private void Awake()
    {
        // get a reference to our main camera
        if (_mainCamera == null)
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }


        inputActions = new PlayerInputs();
        inputActions.Enable();

        movement3D = GetComponent<Movement3D>();
        movement3D.maxSpeed = movementSpeed;


        inputActions.Player.Move.performed += Move2D;
        inputActions.Player.Move.canceled += Move2D;

        inputActions.Player.Jump.performed += Jump;
        inputActions.Player.Jump.canceled += SlowDownJump;

        inputActions.Player.Dive.performed += Dive;
        inputActions.Player.Dive.canceled += SlowDownDive;

        inputActions.Player.Dash.performed += Dash;
        inputActions.Player.Dash.canceled += SlowDownDash;



        // _input = GetComponent<StarterAssetsInputs>();

        // _playerInput = GetComponent<PlayerInput>();


    }

    private void Update()
    {
        jumpCounter -= Time.deltaTime;
        // jumpBufferCounter -= Time.deltaTime;

        dashCounter -= Time.deltaTime;


        isGrounded = movement3D.IsGrounded();
        // print("isGrounded " + isGrounded);

        if (isGrounded)
        {
            dashInMidAirCounter = 0;
            jumpInMidAirCounter = 0;
        }

        if (!isGrounded && jumpInMidAirCounter >= jumpInMidAirMaxNumber)
        {
            isSupposedToJump = false;
        }

        //if (isGrounded && jumpCounter < 0f)
        //{
        //    hangCounter = hangTime;
        //}
        //else
        //{
        //    hangCounter -= Time.deltaTime;
        //}

        // if (jumpBufferCounter < 0f)
        // {
        //     isSupposedToJump = false;
        //     isSupposedToLittleJump = false;
        // }

        // print("jumpCounter < 0f " + (jumpCounter < 0f));
        // print("hangCounter > 0f " + (hangCounter > 0f));
        // print("jumpBufferCounter > 0f " + (jumpBufferCounter > 0f));
        // print("!isJumpingDown " + (!isJumpingDown));

        if (jumpCounter < 0f
            //hangCounter > 0f &&
            // jumpBufferCounter > 0f &&
            // !isJumpingDown)
            && isSupposedToJump)
        {
            if (isSupposedToLittleJump)
            {
                movement3D.Jump(littleJumpForse);
            }
            else
            {
                movement3D.Jump(jumpForse);
            }
            //hangCounter = 0f;
            // jumpBufferCounter = 0f;

            jumpCounter = jumpCoolDown;

            ++jumpInMidAirCounter;

            isSupposedToJump = false;
            isSupposedToLittleJump = false;
        }

        // if (isJumpingDown && isSupposedToStopJumpingDown)// && !movement3D.IsCollidingWithPlatform())
        // {
        //     // gameObject.layer = defaultPlayerLayer;

        //     isJumpingDown = false;
        //     isSupposedToStopJumpingDown = false;
        // }


        if (isDashing)
        {
            dashDurationCounter -= Time.deltaTime;

            if (dashDurationCounter < 0f)
            {
                movement3D.StopDash();
                playerHealth.MakeVulnerable();
                isDashing = false;
            }
        }

        if (dashCounter < 0f && isSupposedToDash)
        {
            if (dashInMidAirCounter < dashInMidAirMaxNumber)
            {
                dashInMidAirCounter++;

                movement3D.Dash(dashSpeedCoef);
                playerHealth.MakeInvincible();

                dashCounter = dashCoolDown;
                dashDurationCounter = dashDuration;

                isDashing = true;
            }

            isSupposedToDash = false;
        }



        Vector2 movement = movement2D;
        //if (isFlying)
        //{
        //    movement = movement2D;
        //    // animator.SetFloat("Speed", 0f);
        //}
        //else
        //{
        //    movement = Vector2.right * movement1D;
        //    // animator.SetFloat("Speed", Mathf.Abs(movement1D));
        //}

        // if (!creatureController.IsFlipLocked())
        // {
        //     if (movement.x > Mathf.Epsilon)
        //     {
        //         movement3D.Flip(true);
        //     }
        //     if (movement.x < -Mathf.Epsilon)
        //     {
        //         movement3D.Flip(false);
        //     }
        // }

        //print(movement);

        movement3D.Move(movement * movementSpeed);
    }
}
