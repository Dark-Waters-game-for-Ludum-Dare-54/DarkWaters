using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{

    public EnemyAttack[] enemyAttackPool;



    public Transform player;


    public Transform intoPosition;

    public Transform[] attackLocations;


    public GameObject debugLightElement;


    private StateMachine stateMachine;



    public CinemachineImpulseSource cinemachineCommonImpulseSource;
    public CinemachineImpulseSource cinemachineDirectImpulseSource;



    public float shakeDirectImpulseAmplitude = 0.1f;
    public float shakeCommonImpulseAmplitude = 0.1f;
    public float shakeDuration = 1f;


    public float introDuration = 0.5f;
    public float moveToNewLocationDuration = 0.5f;

    public float faintDuration = 0.5f;



    public void OnDamageReceived()
    {
        stateMachine.ChangeStateIfAllowed(StateType.Damage);
    }


    private void Start()
    {
        //cinemachineCommonImpulseSource.GenerateImpulse(shakeCommonImpulseAmplitude);
        //cinemachineDirectImpulseSource.GenerateImpulse(directionToPlayer * shakeDirectImpulseAmplitude);

        stateMachine = new StateMachine(this);

        foreach (var state in stateMachine.states)
        {
            state.Value.player = player;
            state.Value.transform = transform;
            state.Value.cinemachineCommonImpulseSource = cinemachineCommonImpulseSource;
            state.Value.cinemachineDirectImpulseSource = cinemachineDirectImpulseSource;
            state.Value.shakeDirectImpulseAmplitude = shakeDirectImpulseAmplitude;
            state.Value.shakeCommonImpulseAmplitude = shakeCommonImpulseAmplitude;
            state.Value.shakeDuration = shakeDuration;
        }

        IntroState introState = stateMachine.states[StateType.Intro] as IntroState;
        introState.intoPosition = intoPosition;
        introState.debugLightElement = debugLightElement;
        introState.introDuration = introDuration;


        MoveToNewLocationState moveToNewLocationState = stateMachine.states[StateType.MoveToNewLocation] as MoveToNewLocationState;
        moveToNewLocationState.attackLocations = attackLocations;
        moveToNewLocationState.moveToNewLocationDuration = moveToNewLocationDuration;


        AttackPreparationState attackPreparationState = stateMachine.states[StateType.AttackPreparation] as AttackPreparationState;
        attackPreparationState.enemyAttackPool = enemyAttackPool;
        //attackPreparationState.attackPreperation = attackPreperation;
        // attackPreparationState.attackPreperationDuration = attackPreperationDuration;

        AttackState attackState = stateMachine.states[StateType.Attack] as AttackState;
        //attackState.attackCollider = attackCollider;
        //attackState.attackPreperation = attackPreperation;
        attackState.enemyAttackPool = enemyAttackPool;
        attackState.debugLightElement = debugLightElement;
        // attackState.attackDuration = attackDuration;

        DamageState damageState = stateMachine.states[StateType.Damage] as DamageState;
        damageState.faintDuration = faintDuration;


        stateMachine.Start();

    }


    private void Update()
    {


    }
}







class StateMachine
{

    public Dictionary<StateType, State> states;

    public MonoBehaviour enemyAI;

    private State currentState;

    private StateSharedData stateSharedData;

    public StateMachine(MonoBehaviour enemyAI)
    {
        this.enemyAI = enemyAI;

        stateSharedData = new StateSharedData();

        states = CreateDictionary();

        currentState = states[StateType.Intro];
    }

    public void Start()
    {
        // Debug.Log("StateMachine Start");
        currentState.Start();
    }



    public Coroutine StartCoroutine(IEnumerator enumerator)
    {
        return enemyAI.StartCoroutine(enumerator);
    }

    public void StopCoroutine(Coroutine coroutine)
    {
        enemyAI.StopCoroutine(coroutine);
    }


    public void ChangeState(StateType stateType)
    {
        currentState = states[stateType];
        currentState.Start();
    }

    public void ChangeStateIfAllowed(StateType stateType)
    {
        if (!currentState.GetCanBeInterrupted())
        {
            return;
        }

        currentState.Stop();

        currentState = states[stateType];

        currentState.Start();
    }

    private Dictionary<StateType, State> CreateDictionary()
    {
        Dictionary<StateType, State> states = new()
        {
            { StateType.Intro, new IntroState(this, stateSharedData) },
            { StateType.MoveToNewLocation, new MoveToNewLocationState(this, stateSharedData) },
            { StateType.AttackPreparation, new AttackPreparationState(this, stateSharedData) },
            { StateType.Attack, new AttackState(this, stateSharedData) },
            { StateType.Damage, new DamageState(this, stateSharedData) },
        };
        return states;
    }
}

class StateSharedData
{
    public int enemyAttackId = 0;
}


enum StateType
{
    Intro,
    MoveToNewLocation,
    AttackPreparation,
    // We can cancel the attack if the player is fast enough
    Attack,
    Damage,
    Dead
}

class State
{

    public Transform player;


    public Transform transform;
    public CinemachineImpulseSource cinemachineCommonImpulseSource;
    public CinemachineImpulseSource cinemachineDirectImpulseSource;
    public float shakeDirectImpulseAmplitude;
    public float shakeCommonImpulseAmplitude;
    public float shakeDuration;


    protected StateMachine stateMachine;
    protected StateSharedData stateSharedData;

    public State(StateMachine stateMachine, StateSharedData stateSharedData)
    {
        this.stateMachine = stateMachine;
        this.stateSharedData = stateSharedData;
    }

    public virtual void Start()
    {
        // Debug.Log("State Start");
    }

    public virtual void Stop()
    {
        // Debug.Log("State Stop");
    }

    public virtual bool GetCanBeInterrupted()
    {
        return false;
    }
}


class IntroState : State
{
    public Transform intoPosition;

    public GameObject debugLightElement;

    public float introDuration;

    private Coroutine coroutine = null;


    public IntroState(StateMachine stateMachine, StateSharedData stateSharedData) : base(stateMachine, stateSharedData)
    {
    }

    public override void Start()
    {
        // Debug.Log("IntroState Start");
        coroutine = stateMachine.StartCoroutine(IntroCoroutine());
    }

    public override void Stop()
    {
        // Debug.Log("IntroState Stop");

        if (coroutine != null)
        {
            debugLightElement.SetActive(false);

            stateMachine.StopCoroutine(coroutine);
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


        debugLightElement.SetActive(false);

        stateMachine.ChangeState(StateType.MoveToNewLocation);

        coroutine = null;
    }
}


class MoveToNewLocationState : State
{
    public Transform[] attackLocations;


    public float moveToNewLocationDuration;


    private Coroutine coroutine = null;

    public MoveToNewLocationState(StateMachine stateMachine, StateSharedData stateSharedData) : base(stateMachine, stateSharedData)
    {
    }

    public override void Start()
    {
        // Debug.Log("MoveToNewLocation Start");
        coroutine = stateMachine.StartCoroutine(MoveToNewLocationCoroutine());
    }

    public override void Stop()
    {
        // Debug.Log("MoveToNewLocation Stop");

        if (coroutine != null)
        {
            stateMachine.StopCoroutine(coroutine);
        }
    }


    private IEnumerator MoveToNewLocationCoroutine()
    {

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

        stateMachine.ChangeState(StateType.AttackPreparation);

        coroutine = null;
    }


    private Vector3 GenRandomLocation()
    {
        int randomIndex = Random.Range(0, attackLocations.Length);
        Vector3 randomLocation = attackLocations[randomIndex].position;

        return randomLocation;
    }
}

class AttackPreparationState : State
{
    public EnemyAttack[] enemyAttackPool;

    private Coroutine coroutine = null;

    public AttackPreparationState(StateMachine stateMachine, StateSharedData stateSharedData) : base(stateMachine, stateSharedData)
    {

    }

    public override void Start()
    {
        // Debug.Log("AttackPreparation Start");
        coroutine = stateMachine.StartCoroutine(AttackPreparationCoroutine());
    }

    public override void Stop()
    {
        // Debug.Log("AttackPreparation Stop");

        if (coroutine != null)
        {
            stateMachine.StopCoroutine(coroutine);
            enemyAttackPool[stateSharedData.enemyAttackId].OnEnemyAttackPreperationInterrupted();
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
        stateSharedData.enemyAttackId = randomIndex;


        float elapsed = 0f;
        float duration = enemyAttackPool[randomIndex].preperationDuration;

        enemyAttackPool[randomIndex].OnEnemyAttackPreperation();

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            yield return null;
        }

        stateMachine.ChangeState(StateType.Attack);
        
        coroutine = null;
    }

}

class AttackState : State
{
    public EnemyAttack[] enemyAttackPool;

    public GameObject debugLightElement;

    private Coroutine coroutine = null;

    public AttackState(StateMachine stateMachine, StateSharedData stateSharedData) : base(stateMachine, stateSharedData)
    {
    }

    public override void Start()
    {
        // Debug.Log("Attack Start");
        coroutine = stateMachine.StartCoroutine(AttackCoroutine());
    }

    public override void Stop()
    {
        // Debug.Log("Attack Stop");

        if (coroutine != null)
        {
            debugLightElement.SetActive(false);

            stateMachine.StopCoroutine(coroutine);
        }
    }

    private IEnumerator AttackCoroutine()
    {
        int enemyAttackId = stateSharedData.enemyAttackId;


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

        stateMachine.ChangeState(StateType.MoveToNewLocation);

        coroutine = null;
    }
}

class DamageState : State
{

    public float faintDuration;


    private Coroutine coroutine = null;

    public DamageState(StateMachine stateMachine, StateSharedData stateSharedData) : base(stateMachine, stateSharedData)
    {
    }

    public override void Start()
    {
        // Debug.Log("Damage Start");
        coroutine = stateMachine.StartCoroutine(DamageCoroutine());
    }

    private IEnumerator DamageCoroutine()
    {
        float elapsed = 0f;


        while (elapsed < faintDuration)
        {
            elapsed += Time.deltaTime;

            yield return null;
        }

        stateMachine.ChangeState(StateType.MoveToNewLocation);

        coroutine = null;
    }
}


class Utilities
{

    public static float EasyOut(float x)
    {
        return 1f - (1f - x) * (1f - x);
    }

    public static float EasyIn(float x)
    {
        return x * x;
    }
}
