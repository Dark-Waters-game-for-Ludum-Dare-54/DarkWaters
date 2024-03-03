using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DamageReceiver : MonoBehaviour
{
    public float damageCooldown = 0.3f;
    public Material woundedMaterial;
    public Renderer characterRenderer; // Reference to the character's renderer.
    public float VFXduration = 0.5f;
    public bool isPlayer;
    public int maxHP = 3;
    public Material hpIndicator;


    public UnityEvent onDamageReceived;

    private float lastDamageReceivedTime = 0f;
    private Material originalMaterial; // Store the original material.
    private int currentHP;

    private void Start ()
    {
        // Get the character's renderer component.
        //characterRenderer = GetComponent<Renderer> ();

        // Make sure the wounded material is assigned initially.
        if (woundedMaterial == null)
        {
            Debug.LogError ("Wounded material is not assigned.");
        }

        if (isPlayer)
        {
            currentHP = maxHP;
            hpIndicator.SetFloat ("_HP", currentHP);
        }
    }


    public void OnDamageReceived()
    {
        if (Time.time - lastDamageReceivedTime > damageCooldown)
        {
            lastDamageReceivedTime = Time.time;

            OnDamage();
        }
    }


    private void OnDamage()
    {
        onDamageReceived.Invoke();

        // Check if the character has a renderer and the wounded material is assigned.
        if (characterRenderer != null && woundedMaterial != null)
        {
            // Store the original material if it hasn't been stored yet.
            if (originalMaterial == null)
            {
                originalMaterial = characterRenderer.material;
            }

            // Change the material to "Wounded."
            characterRenderer.material = woundedMaterial;

            // Start a coroutine to revert to the original material after a delay.
            StartCoroutine (RevertToOriginalMaterial ());
        }
        else
        {
            Debug.LogError ("Character renderer or wounded material not assigned.");
        }

        if (isPlayer)
        {
            print ("Player HP: " + currentHP);
            if (currentHP > 0)
            {
                currentHP--;
                hpIndicator.SetFloat ("_HP", currentHP);
            }
            
        }
        
    }

    // Coroutine to revert to the original material after a delay.
    private IEnumerator RevertToOriginalMaterial ()
    {
        yield return new WaitForSeconds (VFXduration); // Change this delay as needed.

        // Change the material back to the original material.
        if (characterRenderer != null && originalMaterial != null)
        {
            characterRenderer.material = originalMaterial;
        }
    }
}
