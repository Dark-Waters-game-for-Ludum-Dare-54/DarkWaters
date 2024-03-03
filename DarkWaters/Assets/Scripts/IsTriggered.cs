using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class IsTriggered : MonoBehaviour
{
    [SingleLayer]
    public int groundLabel;

    public UnityEvent onTriggerEnter;
    public UnityEvent onTriggerExit;

    [HideInInspector]
    public bool isTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == groundLabel)
        {
            isTriggered = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == groundLabel)
        {
            isTriggered = false;
        }
    }
}
