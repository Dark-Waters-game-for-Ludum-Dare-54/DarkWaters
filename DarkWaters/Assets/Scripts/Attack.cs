using StarterAssets;
using System.Collections;
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
    public SlashVFX slash_vfx;
    public GameObject attackCollider;


    public void Lock()
    {
        inputActions.Disable();
    }

    public void Unlock()
    {
        inputActions.Enable();
    }


    private void Awake()
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
        // Coroutine -> Slash VFX + show the Light
        StartCoroutine (SlashAttack ());
    }

    IEnumerator SlashAttack ()
    {
        // Start the attack with the light intensity at 0
        float startIntensity = 0f;
        float endIntensity = slash_vfx.peakLightIntensity;
        float duration = slash_vfx.duration;
        float elapsedTime = 0f;

        //yield return new WaitForSeconds (slash_vfx.delay);
        slash_vfx.slashObj.SetActive (true);
        attackCollider.SetActive (true);

        // Find the Light component within slash_vfx
        Light slashLight = slash_vfx.slashObj.GetComponentInChildren<Light> ();

        if (slashLight != null)
        {
            // Gradually increase the light intensity from 0 to 3
            while (elapsedTime < duration / 2f)
            {
                slashLight.intensity = Mathf.Lerp (startIntensity, endIntensity, elapsedTime / (duration / 2f));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure the light intensity is exactly 3 at the peak of the attack
            slashLight.intensity = endIntensity;

            // Wait for the second half of the attack duration
            yield return new WaitForSeconds (duration / 2f);

            // Gradually decrease the light intensity from 3 back to 0
            elapsedTime = 0f;
            while (elapsedTime < duration / 2f)
            {
                slashLight.intensity = Mathf.Lerp (endIntensity, startIntensity, elapsedTime / (duration / 2f));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure the light intensity is exactly 0 when the attack is finished
            slashLight.intensity = startIntensity;
        }
        else
        {
            Debug.LogError ("Light component not found under 'slash_vfx'. Make sure your object hierarchy is correct.");
        }

        attackCollider.SetActive (false);
        slash_vfx.slashObj.SetActive (false);
    }
}

[System.Serializable]
public class SlashVFX
{
    public GameObject slashObj;
    public float delay;
    public float duration;
    public float peakLightIntensity;
}
