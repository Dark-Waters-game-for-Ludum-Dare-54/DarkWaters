using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVerticalAttack : EnemyAttack
{
    public GameObject attackPreperation;
    public GameObject attackCollider;


    public override void OnEnemyAttackPreperation()
    {
        attackPreperation.SetActive(true);
        
        StartCoroutine(AttackPreperationCoroutine());
    }

    public override void OnEnemyAttackPreperationInterrupted()
    {
        attackPreperation.SetActive(false);
    }

    public override void OnEnemyAttack()
    {
        attackCollider.SetActive(true);

        StartCoroutine(AttackCoroutine());
    }


    private IEnumerator AttackPreperationCoroutine()
    {
        yield return new WaitForSeconds(preperationDuration);

        attackPreperation.SetActive(false);
    }

    private IEnumerator AttackCoroutine()
    {
        yield return new WaitForSeconds(attackDuration);

        attackCollider.SetActive(false);
    }
}
