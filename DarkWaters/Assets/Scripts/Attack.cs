using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class Attack : MonoBehaviour
{


    public float AttackTimeout = 0.5f;


    private PlayerInputs inputActions;

    private float _attackTimeoutDelta;

    private PlayerInput _playerInput;
    private StarterAssetsInputs _input;


    private void Start()
    {
        inputActions = new PlayerInputs();
        inputActions.Enable();
        
        _attackTimeoutDelta = AttackTimeout;

        inputActions.Player.Attack.performed += onAttackTriggered;
    }


    private void Update()
    {
        _attackTimeoutDelta -= Time.deltaTime;
    }


    private void onAttackTriggered(InputAction.CallbackContext obj)
    {
        if (_attackTimeoutDelta <= 0.0f)
        {
            TriggerOnAttack();
            _attackTimeoutDelta = AttackTimeout;
        }
    }


    private void TriggerOnAttack()
    {
        Debug.Log("Attack!");
    }
}
