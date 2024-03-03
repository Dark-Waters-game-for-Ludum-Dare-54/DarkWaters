using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageReceiver : MonoBehaviour
{
    public float damageCooldown = 0.3f;


    private float lastDamageReceivedTime = 0f;


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
        print("Ah!!!, it hurts!!!");
    }
}
