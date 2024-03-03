using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour
{
    public UnityEvent onEnemyDeath;

    public Material woundedMaterial;
    public Renderer characterRenderer; // Reference to the character's renderer.
    public float VFXduration = 0.5f;

    private Material originalMaterial; // Store the original material.


    public int maxHP = 3;

    private int currentHP;

    public void Revive()
    {
        currentHP = maxHP;
    }

    private void Awake()
    {
        currentHP = maxHP;
    }

    public void OnDamage()
    {
        currentHP--;

        print("Enemy HP: " + currentHP);

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


        if (currentHP == 0)
        {
            onEnemyDeath.Invoke();
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
