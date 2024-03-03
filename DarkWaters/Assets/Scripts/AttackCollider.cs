using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    [SingleLayer]
    public int groundLabel;

    //[HideInInspector]
    //public bool isTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == groundLabel)
        {
            DamageReceiver damageReceiver = other.GetComponent<DamageReceiver>();

            damageReceiver.OnDamageReceived();

            //isTriggered = true;


            //print("OnTriggerEnter: " + isTriggered);
        }
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.gameObject.layer == groundLabel)
    //    {
    //        isTriggered = false;


    //        print("OnTriggerExit: " + isTriggered);
    //    }
    //}
}
