using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class IsTriggered : MonoBehaviour
{
    [SingleLayer]
    public int groundLabel;

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
