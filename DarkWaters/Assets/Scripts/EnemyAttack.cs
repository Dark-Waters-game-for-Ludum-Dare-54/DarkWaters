using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    
    public float preperationDuration = 0.5f;
    public float attackDuration = 0.5f;


    public virtual void OnEnemyAttackPreperation() {

    }

    public virtual void OnEnemyAttackPreperationInterrupted() {

    }

    public virtual void OnEnemyAttack() {

    }
}
