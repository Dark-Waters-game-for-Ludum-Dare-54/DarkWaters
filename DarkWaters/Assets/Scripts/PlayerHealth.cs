using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    public UnityEvent onPlayerDeath;


    public int maxHP = 3;
    public Material hpIndicator;

    private int currentHP;

    private bool isInvincible = false;

    public void MakeInvincible()
    {
        isInvincible = true;
    }

    public void MakeVulnerable()
    {
        isInvincible = false;
    }


    public void Revive()
    {
        currentHP = maxHP;
        hpIndicator.SetFloat("_HP", currentHP);
    }

    private void Awake()
    {
        currentHP = maxHP;
        hpIndicator.SetFloat("_HP", currentHP);
    }

    public void OnDamage()
    {
        if(isInvincible)
        {
            return;
        }

        currentHP--;

        print("Player HP: " + currentHP);
        
        if (currentHP >= 0)
        {
            hpIndicator.SetFloat("_HP", currentHP);
        }

        if (currentHP == 0)
        {
            onPlayerDeath.Invoke();
        }
    }
}
