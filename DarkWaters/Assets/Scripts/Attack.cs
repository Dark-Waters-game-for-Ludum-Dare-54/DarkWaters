using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class Attack : MonoBehaviour
{


    public float AttackTimeout = 0.5f;


    private float _attackTimeoutDelta;

    private PlayerInput _playerInput;
    private StarterAssetsInputs _input;


    private void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        _input = GetComponent<StarterAssetsInputs>();

        _attackTimeoutDelta = AttackTimeout;
    }


    private void Update()
    {
        
        if (_input.attack && _attackTimeoutDelta <= 0.0f)
        {
            TriggerOnAttack();
            _attackTimeoutDelta = AttackTimeout;
            _input.attack = false;
        }

        _attackTimeoutDelta -= Time.deltaTime;
    }



    private void TriggerOnAttack()
    {
        Debug.Log("Attack!");
    }
}
