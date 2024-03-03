using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Movement3D : MonoBehaviour
{
    public IsTriggered isGroundedTrigger;

    public Transform bodyRotation;


    [HideInInspector]
    public float maxSpeed = 1.0f;


    public float stepOffset = 1f;

    public float movementSmoothTime = 0.05f;
    public float airControlSmoothTime = 0.5f;


    [Space(10)]
    public AudioClip LandingAudioClip;
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;
    [Space(10)]


    private Quaternion rotationTarget;
    private Quaternion initialRotation;

    private Rigidbody mainRigidbody;
    private Animator _animator;

    private Vector3 currentVelocity = Vector3.zero;

    private bool isFalling = false;


    private bool isDashing = false;
    private float dashSpeedCoef = 1f;


    private bool isClearTarget = false;
    private Vector3 clearTarget = Vector3.zero;


    private float _animationBlend = 0.0f;


    // animation IDs
    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed;


    public bool IsGrounded()
    {
        if (isGroundedTrigger)
        {
            return isGroundedTrigger.isTriggered;
        }
        return false;
    }

    public void ResetRotation()
    {
        rotationTarget = initialRotation;
        bodyRotation.rotation = rotationTarget;
    }

    public void Move(Vector2 motion)
    {
        isClearTarget = motion.sqrMagnitude > 0.01f;

        Vector3 target = new(motion.x, mainRigidbody.velocity.y, motion.y);

        if (isClearTarget)
        {
            clearTarget = new(motion.x, 0.0f, motion.y);
        }

        if (isDashing)
        {
            target *= dashSpeedCoef;
        }

        float smoothTime;
        if (IsGrounded())
        {
            smoothTime = movementSmoothTime;
        }
        else
        {
            smoothTime = airControlSmoothTime;
        }

        // Vector3 velocity = Vector3.Lerp(mainRigidbody.velocity, target, smoothTime * Time.deltaTime);

        mainRigidbody.velocity = Vector3.SmoothDamp(mainRigidbody.velocity, target, ref currentVelocity, smoothTime);

        if (isClearTarget)
        {
            rotationTarget = Quaternion.LookRotation(new Vector3(motion.x, 0.0f, motion.y).normalized, Vector3.up);
        }

        bodyRotation.rotation = Quaternion.Slerp(bodyRotation.rotation, rotationTarget, 10.0f * Time.deltaTime);
        //print("bodyRotation.rotation " + bodyRotation.rotation);

        _animationBlend = Mathf.Lerp(_animationBlend, mainRigidbody.velocity.sqrMagnitude / 2.0f, Time.deltaTime * 10.0f);
        if (_animationBlend < 0.01f)
        {
            _animationBlend = 0f;
        }

        float inputMagnitude = motion.magnitude / maxSpeed;

        if (_animator)
        {
            _animator.SetFloat(_animIDSpeed, _animationBlend);
            _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
        }
    }

    public void Jump(float force)
    {
        if (_animator)
        {
            _animator.SetBool(_animIDJump, true);
            _animator.SetBool(_animIDFreeFall, false);
        }

        mainRigidbody.velocity = new Vector3(mainRigidbody.velocity.x, force, mainRigidbody.velocity.z);
    }

    public void SlowDownJump()
    {

        if (mainRigidbody.velocity.y > 0f)
        {
            mainRigidbody.velocity = new Vector3(mainRigidbody.velocity.x, mainRigidbody.velocity.y / 2f, mainRigidbody.velocity.z);
        }
    }

    public void Dive(float force)
    {
        if (_animator)
        {
            _animator.SetBool(_animIDJump, false);
            _animator.SetBool(_animIDFreeFall, true);
        }

        mainRigidbody.velocity = new Vector3(mainRigidbody.velocity.x, -force, mainRigidbody.velocity.z);
    }

    public void Dash(float newDashSpeedCoef)
    {
        dashSpeedCoef = newDashSpeedCoef;
        isDashing = true;

        if (isClearTarget)
        {
            currentVelocity = clearTarget * dashSpeedCoef;

            mainRigidbody.velocity = clearTarget * dashSpeedCoef;
        }
        else
        {
            currentVelocity = new Vector3(currentVelocity.x * dashSpeedCoef, 0.0f, currentVelocity.z * dashSpeedCoef);

            mainRigidbody.velocity = new Vector3(mainRigidbody.velocity.x * dashSpeedCoef, 0.0f, mainRigidbody.velocity.z * dashSpeedCoef);
        }
    }

    public void StopDash()
    {
        isDashing = false;

        currentVelocity = new Vector3(currentVelocity.x / dashSpeedCoef, 0.0f, currentVelocity.z / dashSpeedCoef);

        mainRigidbody.velocity = new Vector3(mainRigidbody.velocity.x / dashSpeedCoef, 0.0f, mainRigidbody.velocity.z / dashSpeedCoef);
    }

    private void Awake()
    {
        mainRigidbody = GetComponent<Rigidbody>();

        rotationTarget = bodyRotation.rotation;
        initialRotation = bodyRotation.rotation;
    }

    private void Start()
    {
        TryGetComponent(out _animator);

        AssignAnimationIDs();
    }

    private void Update()
    {
        if (mainRigidbody.velocity.y < -0.001)
        {
            isFalling = true;
            if (_animator)
            {
                _animator.SetBool(_animIDJump, false);
                _animator.SetBool(_animIDFreeFall, true);
            }
        }

        if (Mathf.Abs(mainRigidbody.velocity.y) < 0.001)
        {
            if (isFalling)
            {
                isFalling = false;
            }

            if (_animator)
            {
                _animator.SetBool(_animIDJump, false);
                _animator.SetBool(_animIDFreeFall, false);
            }
        }

        if (_animator)
        {
            _animator.SetBool(_animIDGrounded, IsGrounded());
        }
    }

    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    private void OnFootstep(AnimationEvent animationEvent)
    {
        // if (animationEvent.animatorClipInfo.weight > 0.5f)
        // {
        //     if (FootstepAudioClips.Length > 0)
        //     {
        //         var index = Random.Range(0, FootstepAudioClips.Length);
        //         AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.position, FootstepAudioVolume);
        //     }
        // }
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        // if (animationEvent.animatorClipInfo.weight > 0.5f)
        // {
        //     // AudioSource.PlayClipAtPoint(LandingAudioClip, transform.position, FootstepAudioVolume);
        // }
    }
}
