using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{

    public EnemyAttack[] enemyAttackPool;



    public Transform player;


    public Transform intoPosition;
    public Transform deathPosition;

    public Transform[] attackLocations;


    public GameObject debugLightElement;


    private EnemyStateMachine enemyStateMachine;



    public CinemachineImpulseSource cinemachineCommonImpulseSource;
    public CinemachineImpulseSource cinemachineDirectImpulseSource;



    public float shakeDirectImpulseAmplitude = 2.0f;
    public float shakeCommonImpulseAmplitude = 50.0f;
    public float shakeDuration = 0.1f;


    public float introDuration = 0.5f;
    public float moveToNewLocationDuration = 0.5f;

    public float faintDuration = 0.5f;



    private bool isLocked = false;


    public void OnDamageReceived()
    {
        if (isLocked)
        {
            return;
        }

        enemyStateMachine.ChangeEnemyStateIfAllowed(EnemyStateType.Damage);
    }

    public void OnDeath()
    {
        enemyStateMachine.ChangeEnemyState(EnemyStateType.Dead);
    }

    public void Lock()
    {
        isLocked = true;
        enemyStateMachine.Stop();
    }

    public void Unlock()
    {
        isLocked = false;

        enemyStateMachine.ChangeEnemyState(EnemyStateType.Intro);
    }

    public void StartBattle()
    {
        enemyStateMachine.ChangeEnemyState(EnemyStateType.MoveToNewLocation);
    }

    public void StartStandingOverPlayersCorpse()
    {
        enemyStateMachine.ChangeEnemyState(EnemyStateType.StandingOverPlayersCorpse);
    }


    private void Awake()
    {
        //cinemachineCommonImpulseSource.GenerateImpulse(shakeCommonImpulseAmplitude);
        //cinemachineDirectImpulseSource.GenerateImpulse(directionToPlayer * shakeDirectImpulseAmplitude);

        enemyStateMachine = new EnemyStateMachine(this);

        foreach (var enemyState in enemyStateMachine.enemyStates)
        {
            enemyState.Value.player = player;
            enemyState.Value.transform = transform;
            enemyState.Value.cinemachineCommonImpulseSource = cinemachineCommonImpulseSource;
            enemyState.Value.cinemachineDirectImpulseSource = cinemachineDirectImpulseSource;
            enemyState.Value.shakeDirectImpulseAmplitude = shakeDirectImpulseAmplitude;
            enemyState.Value.shakeCommonImpulseAmplitude = shakeCommonImpulseAmplitude;
            enemyState.Value.shakeDuration = shakeDuration;
        }

        IntroEnemyState introEnemyState = enemyStateMachine.enemyStates[EnemyStateType.Intro] as IntroEnemyState;
        introEnemyState.intoPosition = intoPosition;
        introEnemyState.debugLightElement = debugLightElement;
        introEnemyState.introDuration = introDuration;


        MoveToNewLocationEnemyState moveToNewLocationEnemyState = enemyStateMachine.enemyStates[EnemyStateType.MoveToNewLocation] as MoveToNewLocationEnemyState;
        moveToNewLocationEnemyState.attackLocations = attackLocations;
        moveToNewLocationEnemyState.moveToNewLocationDuration = moveToNewLocationDuration;
        moveToNewLocationEnemyState.debugLightElement = debugLightElement;


        AttackPreparationEnemyState attackPreparationEnemyState = enemyStateMachine.enemyStates[EnemyStateType.AttackPreparation] as AttackPreparationEnemyState;
        attackPreparationEnemyState.enemyAttackPool = enemyAttackPool;
        //attackPreparationEnemyState.attackPreperation = attackPreperation;
        // attackPreparationEnemyState.attackPreperationDuration = attackPreperationDuration;

        AttackEnemyState attackEnemyState = enemyStateMachine.enemyStates[EnemyStateType.Attack] as AttackEnemyState;
        //attackEnemyState.attackCollider = attackCollider;
        //attackEnemyState.attackPreperation = attackPreperation;
        attackEnemyState.enemyAttackPool = enemyAttackPool;
        attackEnemyState.debugLightElement = debugLightElement;
        // attackEnemyState.attackDuration = attackDuration;

        DamageEnemyState damageEnemyState = enemyStateMachine.enemyStates[EnemyStateType.Damage] as DamageEnemyState;
        damageEnemyState.faintDuration = faintDuration;

        DeadEnemyState deadEnemyState = enemyStateMachine.enemyStates[EnemyStateType.Dead] as DeadEnemyState;
        deadEnemyState.deathPosition = deathPosition;
        deadEnemyState.debugLightElement = debugLightElement;


        StandingOverPlayersCorpseEnemyState standingOverPlayersCorpseEnemyState = enemyStateMachine.enemyStates[EnemyStateType.StandingOverPlayersCorpse] as StandingOverPlayersCorpseEnemyState;
        standingOverPlayersCorpseEnemyState.debugLightElement = debugLightElement;


    }


    private void Update()
    {


    }
}







class EnemyStateMachine
{

    public Dictionary<EnemyStateType, EnemyState> enemyStates;

    public MonoBehaviour enemyAI;

    private EnemyState currentEnemyState;

    private EnemyStateSharedData enemyStateSharedData;

    public EnemyStateMachine(MonoBehaviour enemyAI)
    {
        this.enemyAI = enemyAI;

        enemyStateSharedData = new EnemyStateSharedData();

        enemyStates = CreateDictionary();

        currentEnemyState = enemyStates[EnemyStateType.Intro];
    }

    public void Start()
    {
        // Debug.Log("EnemyStateMachine Start");
        currentEnemyState.Start();
    }



    public Coroutine StartCoroutine(IEnumerator enumerator)
    {
        return enemyAI.StartCoroutine(enumerator);
    }

    public void StopCoroutine(Coroutine coroutine)
    {
        enemyAI.StopCoroutine(coroutine);
    }


    public void ChangeEnemyState(EnemyStateType enemyStateType)
    {
        currentEnemyState = enemyStates[enemyStateType];
        currentEnemyState.Start();
    }

    public void ChangeEnemyStateIfAllowed(EnemyStateType enemyStateType)
    {
        if (!currentEnemyState.GetCanBeInterrupted())
        {
            return;
        }

        currentEnemyState.Stop();

        currentEnemyState = enemyStates[enemyStateType];

        currentEnemyState.Start();
    }

    public void ChangeEnemyStateForced(EnemyStateType enemyStateType)
    {
        currentEnemyState.Stop();

        currentEnemyState = enemyStates[enemyStateType];

        currentEnemyState.Start();
    }

    public void Stop()
    {
        // Debug.Log("EnemyStateMachine Stop");
        currentEnemyState.Stop();
        currentEnemyState = enemyStates[EnemyStateType.None];
    }

    private Dictionary<EnemyStateType, EnemyState> CreateDictionary()
    {
        Dictionary<EnemyStateType, EnemyState> enemyStates = new()
        {
            { EnemyStateType.Intro, new IntroEnemyState(this, enemyStateSharedData) },
            { EnemyStateType.MoveToNewLocation, new MoveToNewLocationEnemyState(this, enemyStateSharedData) },
            { EnemyStateType.AttackPreparation, new AttackPreparationEnemyState(this, enemyStateSharedData) },
            { EnemyStateType.Attack, new AttackEnemyState(this, enemyStateSharedData) },
            { EnemyStateType.Damage, new DamageEnemyState(this, enemyStateSharedData) },
            { EnemyStateType.Dead, new DeadEnemyState(this, enemyStateSharedData) },
            { EnemyStateType.StandingOverPlayersCorpse, new StandingOverPlayersCorpseEnemyState(this, enemyStateSharedData) },
            { EnemyStateType.None, new EnemyState(this, enemyStateSharedData) }
        };
        return enemyStates;
    }
}

class EnemyStateSharedData
{
    public int enemyAttackId = 0;
}


enum EnemyStateType
{
    Intro,
    MoveToNewLocation,
    AttackPreparation,
    // We can cancel the attack if the player is fast enough
    Attack,
    Damage,
    Dead,
    StandingOverPlayersCorpse,
    None
}

class EnemyState
{

    public Transform player;


    public Transform transform;
    public CinemachineImpulseSource cinemachineCommonImpulseSource;
    public CinemachineImpulseSource cinemachineDirectImpulseSource;
    public float shakeDirectImpulseAmplitude;
    public float shakeCommonImpulseAmplitude;
    public float shakeDuration;


    protected EnemyStateMachine enemyStateMachine;
    protected EnemyStateSharedData enemyStateSharedData;

    public EnemyState(EnemyStateMachine enemyStateMachine, EnemyStateSharedData enemyStateSharedData)
    {
        this.enemyStateMachine = enemyStateMachine;
        this.enemyStateSharedData = enemyStateSharedData;
    }

    public virtual void Start()
    {
        // Debug.Log("EnemyState Start");
    }

    public virtual void Stop()
    {
        // Debug.Log("EnemyState Stop");
    }

    public virtual bool GetCanBeInterrupted()
    {
        return false;
    }
}


class IntroEnemyState : EnemyState
{
    public Transform intoPosition;

    public GameObject debugLightElement;

    public float introDuration;

    private Coroutine coroutine = null;


    public IntroEnemyState(EnemyStateMachine enemyStateMachine, EnemyStateSharedData enemyStateSharedData) : base(enemyStateMachine, enemyStateSharedData)
    {
    }

    public override void Start()
    {
        // Debug.Log("IntroEnemyState Start");
        coroutine = enemyStateMachine.StartCoroutine(IntroCoroutine());
    }

    public override void Stop()
    {
        // Debug.Log("IntroEnemyState Stop");

        if (coroutine != null)
        {
            debugLightElement.SetActive(false);

            enemyStateMachine.StopCoroutine(coroutine);
        }
    }

    private IEnumerator IntroCoroutine()
    {
        float elapsed = 0f;

        Vector3 fromPosition = intoPosition.position + new Vector3(0.0f, 50.0f, 0.0f);

        Vector3 toPosition = intoPosition.position;

        debugLightElement.SetActive(true);


        while (elapsed < introDuration)
        {
            float x = elapsed / introDuration;
            x = Utilities.EasyIn(x);
            x = Utilities.EasyIn(x);

            transform.position = Vector3.Lerp(fromPosition, toPosition, x);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.position = toPosition;
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        cinemachineCommonImpulseSource.GenerateImpulse(shakeCommonImpulseAmplitude);
        cinemachineDirectImpulseSource.GenerateImpulse(Vector3.down * shakeDirectImpulseAmplitude);


        elapsed = 0f;
        while (elapsed < 0.3f)
        {
            elapsed += Time.deltaTime;

            yield return null;
        }


        // debugLightElement.SetActive(false);

        // enemyStateMachine.ChangeEnemyState(EnemyStateType.MoveToNewLocation);

        coroutine = null;
    }
}


class MoveToNewLocationEnemyState : EnemyState
{
    public Transform[] attackLocations;

    public GameObject debugLightElement;

    public float moveToNewLocationDuration;


    private Coroutine coroutine = null;

    public MoveToNewLocationEnemyState(EnemyStateMachine enemyStateMachine, EnemyStateSharedData enemyStateSharedData) : base(enemyStateMachine, enemyStateSharedData)
    {
    }

    public override void Start()
    {
        // Debug.Log("MoveToNewLocation Start");
        coroutine = enemyStateMachine.StartCoroutine(MoveToNewLocationCoroutine());
    }

    public override void Stop()
    {
        // Debug.Log("MoveToNewLocation Stop");

        if (coroutine != null)
        {
            enemyStateMachine.StopCoroutine(coroutine);
        }
    }


    private IEnumerator MoveToNewLocationCoroutine()
    {
        debugLightElement.SetActive(false);

        float goingUpDuration = moveToNewLocationDuration / 2.0f;

        float elapsed = 0f;

        Vector3 fromPosition = transform.position;
        Vector3 toPosition = transform.position + new Vector3(0.0f, 50.0f, 0.0f);


        while (elapsed < goingUpDuration)
        {
            float x = elapsed / goingUpDuration;
            x = Utilities.EasyOut(x);
            x = Utilities.EasyOut(x);

            transform.position = Vector3.Lerp(fromPosition, toPosition, x);

            elapsed += Time.deltaTime;

            yield return null;
        }

        Vector3 randomLocation = GenRandomLocation();

        float goingDownDuration = moveToNewLocationDuration / 2.0f;

        elapsed = 0f;

        fromPosition = randomLocation + new Vector3(0.0f, 50.0f, 0.0f);
        toPosition = randomLocation;


        while (elapsed < goingDownDuration)
        {
            float x = elapsed / goingDownDuration;
            x = Utilities.EasyIn(x);
            x = Utilities.EasyIn(x);

            transform.position = Vector3.Lerp(fromPosition, toPosition, x);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.position = toPosition;

        enemyStateMachine.ChangeEnemyState(EnemyStateType.AttackPreparation);

        coroutine = null;
    }


    private Vector3 GenRandomLocation()
    {
        int randomIndex = Random.Range(0, attackLocations.Length);
        Vector3 randomLocation = attackLocations[randomIndex].position;

        return randomLocation;
    }
}

class AttackPreparationEnemyState : EnemyState
{
    public EnemyAttack[] enemyAttackPool;

    private Coroutine coroutine = null;

    public AttackPreparationEnemyState(EnemyStateMachine enemyStateMachine, EnemyStateSharedData enemyStateSharedData) : base(enemyStateMachine, enemyStateSharedData)
    {

    }

    public override void Start()
    {
        // Debug.Log("AttackPreparation Start");
        coroutine = enemyStateMachine.StartCoroutine(AttackPreparationCoroutine());
    }

    public override void Stop()
    {
        // Debug.Log("AttackPreparation Stop");

        if (coroutine != null)
        {
            enemyStateMachine.StopCoroutine(coroutine);
            enemyAttackPool[enemyStateSharedData.enemyAttackId].OnEnemyAttackPreperationInterrupted();
        }
    }

    public override bool GetCanBeInterrupted()
    {
        return true;
    }

    private IEnumerator AttackPreparationCoroutine()
    {
        Vector3 directionToPlayer = Vector3.ProjectOnPlane(player.position - transform.position, Vector3.up).normalized;


        transform.rotation = Quaternion.LookRotation(directionToPlayer, Vector3.up);

        int randomIndex = Random.Range(0, enemyAttackPool.Length);
        enemyStateSharedData.enemyAttackId = randomIndex;


        float elapsed = 0f;
        float duration = enemyAttackPool[randomIndex].preperationDuration;

        enemyAttackPool[randomIndex].OnEnemyAttackPreperation();

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            yield return null;
        }

        enemyStateMachine.ChangeEnemyState(EnemyStateType.Attack);

        coroutine = null;
    }

}

class AttackEnemyState : EnemyState
{
    public EnemyAttack[] enemyAttackPool;

    public GameObject debugLightElement;

    private Coroutine coroutine = null;

    public AttackEnemyState(EnemyStateMachine enemyStateMachine, EnemyStateSharedData enemyStateSharedData) : base(enemyStateMachine, enemyStateSharedData)
    {
    }

    public override void Start()
    {
        // Debug.Log("Attack Start");
        coroutine = enemyStateMachine.StartCoroutine(AttackCoroutine());
    }

    public override void Stop()
    {
        // Debug.Log("Attack Stop");

        if (coroutine != null)
        {
            debugLightElement.SetActive(false);

            enemyStateMachine.StopCoroutine(coroutine);
        }
    }

    private IEnumerator AttackCoroutine()
    {
        int enemyAttackId = enemyStateSharedData.enemyAttackId;


        float elapsed = 0f;
        float duration = enemyAttackPool[enemyAttackId].attackDuration;


        enemyAttackPool[enemyAttackId].OnEnemyAttack();


        debugLightElement.SetActive(true);


        cinemachineCommonImpulseSource.GenerateImpulse(shakeCommonImpulseAmplitude);
        cinemachineDirectImpulseSource.GenerateImpulse(transform.forward * shakeDirectImpulseAmplitude);


        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            yield return null;
        }

        debugLightElement.SetActive(false);



        enemyStateMachine.ChangeEnemyState(EnemyStateType.MoveToNewLocation);

        coroutine = null;
    }
}

class DamageEnemyState : EnemyState
{

    public float faintDuration;


    private Coroutine coroutine = null;

    public DamageEnemyState(EnemyStateMachine enemyStateMachine, EnemyStateSharedData enemyStateSharedData) : base(enemyStateMachine, enemyStateSharedData)
    {
    }

    public override void Start()
    {
        // Debug.Log("Damage Start");
        coroutine = enemyStateMachine.StartCoroutine(DamageCoroutine());
    }

    public override void Stop()
    {
        // Debug.Log("Damage Stop");

        if (coroutine != null)
        {
            enemyStateMachine.StopCoroutine(coroutine);
        }
    }

    private IEnumerator DamageCoroutine()
    {
        float elapsed = 0f;


        while (elapsed < faintDuration)
        {
            elapsed += Time.deltaTime;

            yield return null;
        }

        enemyStateMachine.ChangeEnemyState(EnemyStateType.MoveToNewLocation);

        coroutine = null;
    }
}


class DeadEnemyState : EnemyState
{
    public Transform deathPosition;

    public GameObject debugLightElement;

    private Coroutine coroutine = null;

    public DeadEnemyState(EnemyStateMachine enemyStateMachine, EnemyStateSharedData enemyStateSharedData) : base(enemyStateMachine, enemyStateSharedData)
    {
    }

    public override void Start()
    {
        // Debug.Log("Dead Start");
        coroutine = enemyStateMachine.StartCoroutine(DeadCoroutine());
    }

    private IEnumerator DeadCoroutine()
    {
        debugLightElement.SetActive(false);

        float goingUpDuration = 0.5f;

        float elapsed = 0f;

        Vector3 fromPosition = transform.position;
        Vector3 toPosition = transform.position + new Vector3(0.0f, 50.0f, 0.0f);


        while (elapsed < goingUpDuration)
        {
            float x = elapsed / goingUpDuration;
            x = Utilities.EasyOut(x);
            x = Utilities.EasyOut(x);

            transform.position = Vector3.Lerp(fromPosition, toPosition, x);

            elapsed += Time.deltaTime;

            yield return null;
        }

        
        debugLightElement.SetActive(true);

        Vector3 targetPosiion = deathPosition.position + new Vector3(0.0f, 0.0f, 1.0f);


        Vector3 directionToPlayer = Vector3.ProjectOnPlane(player.position - targetPosiion, Vector3.up).normalized;
        transform.rotation = Quaternion.LookRotation(Vector3.back, Vector3.up);


        float goingDownDuration = 0.5f;

        elapsed = 0f;

        fromPosition = targetPosiion + new Vector3(0.0f, 50.0f, 0.0f);
        toPosition = targetPosiion;


        while (elapsed < goingDownDuration)
        {
            float x = elapsed / goingDownDuration;
            x = Utilities.EasyIn(x);
            x = Utilities.EasyIn(x);

            transform.position = Vector3.Lerp(fromPosition, toPosition, x);

            elapsed += Time.deltaTime;

            yield return null;
        }


        transform.position = toPosition;

        cinemachineCommonImpulseSource.GenerateImpulse(shakeCommonImpulseAmplitude);
        cinemachineDirectImpulseSource.GenerateImpulse(Vector3.down * shakeDirectImpulseAmplitude * 2.0f);



        yield return new WaitForSeconds(2.0f);


        float fromIntensity = debugLightElement.GetComponent<Light>().intensity;
        float toIntensity = 0.0f;

        float lightDownDuration = 2.0f;

        elapsed = 0f;
        

        while (elapsed < lightDownDuration)
        {
            float x = elapsed / lightDownDuration;
            x = Utilities.EasyInOut(x);
            x = Utilities.EasyInOut(x);

            debugLightElement.GetComponent<Light>().intensity = Mathf.Lerp(fromIntensity, toIntensity, x);

            elapsed += Time.deltaTime;

            yield return null;
        }


        debugLightElement.SetActive(false);
        debugLightElement.GetComponent<Light>().intensity = fromIntensity;


        enemyStateMachine.ChangeEnemyState(EnemyStateType.None);

        coroutine = null;
    }
}


class StandingOverPlayersCorpseEnemyState : EnemyState
{

    public GameObject debugLightElement;

    private Coroutine coroutine = null;

    public StandingOverPlayersCorpseEnemyState(EnemyStateMachine enemyStateMachine, EnemyStateSharedData enemyStateSharedData) : base(enemyStateMachine, enemyStateSharedData)
    {
    }

    public override void Start()
    {
        // Debug.Log("StandingOverPlayersCorpse Start");
        coroutine = enemyStateMachine.StartCoroutine(StandingOverPlayersCorpseCoroutine());
    }

    public override void Stop()
    {
        // Debug.Log("StandingOverPlayersCorpse Stop");

        if (coroutine != null)
        {
            enemyStateMachine.StopCoroutine(coroutine);
        }
    }

    private IEnumerator StandingOverPlayersCorpseCoroutine()
    {
        debugLightElement.SetActive(false);

        float goingUpDuration = 0.5f;

        float elapsed = 0f;

        Vector3 fromPosition = transform.position;
        Vector3 toPosition = transform.position + new Vector3(0.0f, 50.0f, 0.0f);


        while (elapsed < goingUpDuration)
        {
            float x = elapsed / goingUpDuration;
            x = Utilities.EasyOut(x);
            x = Utilities.EasyOut(x);

            transform.position = Vector3.Lerp(fromPosition, toPosition, x);

            elapsed += Time.deltaTime;

            yield return null;
        }

        
        debugLightElement.SetActive(true);

        Vector3 targetPosiion = player.position + new Vector3(0.0f, 0.0f, 1.0f);


        Vector3 directionToPlayer = Vector3.ProjectOnPlane(player.position - targetPosiion, Vector3.up).normalized;
        transform.rotation = Quaternion.LookRotation(directionToPlayer, Vector3.up);


        float goingDownDuration = 0.5f;

        elapsed = 0f;

        fromPosition = targetPosiion + new Vector3(0.0f, 50.0f, 0.0f);
        toPosition = targetPosiion;


        while (elapsed < goingDownDuration)
        {
            float x = elapsed / goingDownDuration;
            x = Utilities.EasyIn(x);
            x = Utilities.EasyIn(x);

            transform.position = Vector3.Lerp(fromPosition, toPosition, x);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.position = toPosition;

        cinemachineCommonImpulseSource.GenerateImpulse(shakeCommonImpulseAmplitude);
        cinemachineDirectImpulseSource.GenerateImpulse(Vector3.down * shakeDirectImpulseAmplitude * 2.0f);


        enemyStateMachine.ChangeEnemyState(EnemyStateType.None);

        coroutine = null;
    }
}



public class Utilities
{

    public static float EasyOut(float x)
    {
        return 1f - (1f - x) * (1f - x);
    }

    public static float EasyIn(float x)
    {
        return x * x;
    }

    public static float EasyInOut(float x)
    {
        return -2.0f * x * x * x + 3.0f * x * x;
    }
}
