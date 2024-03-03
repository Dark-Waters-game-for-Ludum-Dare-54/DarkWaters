using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DamageReceiver : MonoBehaviour
{
    public float damageCooldown = 0.3f;

    public UnityEvent onDamageReceived;


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
        onDamageReceived.Invoke();
        // print("Ah!!!, it hurts!!!");
    }
}
