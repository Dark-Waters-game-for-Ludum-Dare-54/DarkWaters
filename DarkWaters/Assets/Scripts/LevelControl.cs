using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelControl : MonoBehaviour
{

    public bool showQuickGuide = true;


    public CharacterMovement characterMovement;
    public Attack attack;
    public PlayerHealth playerHealth;
    public Movement3D movement3D;
    public TrailRenderer[] trs;

    public EnemyAI enemyAI;
    public EnemyHealth enemyHealth;

    public CinemachineImpulseSource cinemachineCommonImpulseSource;
    public CinemachineImpulseSource cinemachineDirectImpulseSource;

    public float shakeDirectImpulseAmplitude = 4.0f;
    public float shakeCommonImpulseAmplitude = 50.0f;
    public float shakeDuration = 0.1f;



    public CinemachineBrain cinemachineBrain;
    public GameObject playerFollowCamera;

    public Transform introTransform;
    public float intoHeight = 100f;

    public IsTriggered isGrounded;

    public CanvasGroup canvasGroup;
    public float guideAppearingDuration = 0.2f;
    public float guideDisappearingDuration = 0.2f;


    public GameObject restartUIObj;
    public GameObject gameOverUIObj;
    public GameObject youWinUIObj;


    public Material restartUI;
    public Material gameOverUI;
    public Material youWinUI;


    public Transform deathPosition;


    private LevelInputs inputActions;


    private LevelStateMachine levelStateMachine;




    public void OnPlayerDeath()
    {
        levelStateMachine.ChangeState(LevelStateType.UserDeath);
    }

    public void OnEnemyDeath()
    {
        levelStateMachine.ChangeState(LevelStateType.EnemyDeath);
    }


    private void Awake()
    {

        Cursor.visible = false;

        inputActions = new LevelInputs();
        inputActions.Enable();


        levelStateMachine = new LevelStateMachine(this, showQuickGuide);

        foreach (var state in levelStateMachine.states)
        {
            state.Value.characterMovement = characterMovement;
            state.Value.enemyAI = enemyAI;
            state.Value.enemyHealth = enemyHealth;
            state.Value.attack = attack;
            state.Value.playerHealth = playerHealth;
            state.Value.movement3D = movement3D;
            state.Value.trs = trs;
            state.Value.playerTransform = characterMovement.transform;
            state.Value.enemyTransform = enemyAI.transform;
            state.Value.cinemachineCommonImpulseSource = cinemachineCommonImpulseSource;
            state.Value.cinemachineDirectImpulseSource = cinemachineDirectImpulseSource;
            state.Value.shakeDirectImpulseAmplitude = shakeDirectImpulseAmplitude;
            state.Value.shakeCommonImpulseAmplitude = shakeCommonImpulseAmplitude;
            state.Value.shakeDuration = shakeDuration;
            state.Value.inputActions = inputActions;
            state.Value.restartUIObj = restartUIObj;
            state.Value.gameOverUIObj = gameOverUIObj;
            state.Value.youWinUIObj = youWinUIObj;
            state.Value.restartUI = restartUI;
            state.Value.gameOverUI = gameOverUI;
            state.Value.youWinUI = youWinUI;
        }

        LevelStateIntro intro = (LevelStateIntro)levelStateMachine.states[LevelStateType.Intro];
        intro.intoHeight = intoHeight;
        intro.isGrounded = isGrounded;
        intro.playerFollowCamera = playerFollowCamera;
        intro.cinemachineBrain = cinemachineBrain;
        intro.introTransform = introTransform;

        LevelStateQuickGuide quickGuide = (LevelStateQuickGuide)levelStateMachine.states[LevelStateType.QuickGuide];
        quickGuide.canvasGroup = canvasGroup;
        quickGuide.guideAppearingDuration = guideAppearingDuration;
        quickGuide.guideDisappearingDuration = guideDisappearingDuration;

        LevelStateUserDeath userDeath = (LevelStateUserDeath)levelStateMachine.states[LevelStateType.UserDeath];
        userDeath.playerFollowCamera = playerFollowCamera;
        userDeath.cinemachineBrain = cinemachineBrain;
        userDeath.introTransform = introTransform;
        userDeath.intoHeight = intoHeight;

        LevelStateEnemyDeath enemyDeath = (LevelStateEnemyDeath)levelStateMachine.states[LevelStateType.EnemyDeath];
        enemyDeath.playerFollowCamera = playerFollowCamera;
        enemyDeath.cinemachineBrain = cinemachineBrain;
        enemyDeath.deathPosition = deathPosition;
        enemyDeath.introTransform = introTransform;
        enemyDeath.intoHeight = intoHeight;

    }


    private void Start()
    {
        levelStateMachine.Start();
    }




}



class LevelStateMachine
{
    public Dictionary<LevelStateType, LevelState> states;

    public MonoBehaviour owner;


    public LevelState currentState;

    private readonly LevelStateSharedData sharedData;

    public LevelStateMachine(MonoBehaviour owner, bool showQuickGuide)
    {
        this.owner = owner;

        sharedData = new LevelStateSharedData
        {
            isQuickGuideShown = !showQuickGuide
        };

        states = CreateStates();

        currentState = states[LevelStateType.Intro];

        // currentState = states[LevelStateType.UserDeath];
        // currentState = states[LevelStateType.EnemyDeath];
    }


    public void Start()
    {
        currentState.Start();
    }


    public Coroutine StartCoroutine(IEnumerator enumerator)
    {
        return owner.StartCoroutine(enumerator);
    }

    public void StopCoroutine(Coroutine coroutine)
    {
        owner.StopCoroutine(coroutine);
    }

    public void ChangeState(LevelStateType type)
    {
        currentState = states[type];

        currentState.Start();
    }

    private Dictionary<LevelStateType, LevelState> CreateStates()
    {
        Dictionary<LevelStateType, LevelState> states = new()
        {
            { LevelStateType.Intro, new LevelStateIntro(this, sharedData) },
            { LevelStateType.QuickGuide, new LevelStateQuickGuide(this, sharedData) },
            { LevelStateType.Play, new LevelStatePlay(this, sharedData) },
            { LevelStateType.UserDeath, new LevelStateUserDeath(this, sharedData) },
            { LevelStateType.EnemyDeath, new LevelStateEnemyDeath(this, sharedData)},
        };
        // states.Add(LevelStateType.EnemyDeath, new LevelStateEnemyDeath(this, sharedData));
        // states.Add(LevelStateType.Outro, new LevelStateOutro(this, sharedData));

        return states;
    }
}


class LevelStateSharedData
{
    public bool isQuickGuideShown = false;
}

enum LevelStateType
{
    Intro,
    QuickGuide,
    Play,
    UserDeath,
    EnemyDeath,
    Outro,
}

class LevelState
{
    public CharacterMovement characterMovement;

    public EnemyAI enemyAI;
        public EnemyHealth enemyHealth;
    public PlayerHealth playerHealth;
        public Movement3D movement3D;
    public Attack attack;

    public TrailRenderer[] trs;

    public Transform playerTransform;
    public Transform enemyTransform;

    public CinemachineImpulseSource cinemachineCommonImpulseSource;
    public CinemachineImpulseSource cinemachineDirectImpulseSource;
    public float shakeDirectImpulseAmplitude;
    public float shakeCommonImpulseAmplitude;
    public float shakeDuration;

    public LevelInputs inputActions;

    public GameObject restartUIObj;
    public GameObject gameOverUIObj;
    public GameObject youWinUIObj;

    public Material restartUI;
    public Material gameOverUI;
    public Material youWinUI;

    protected LevelStateMachine stateMachine;

    protected LevelStateSharedData sharedData;

    public LevelState(LevelStateMachine stateMachine, LevelStateSharedData sharedData)
    {
        this.stateMachine = stateMachine;
        this.sharedData = sharedData;
    }

    public virtual void Start()
    {

    }

    public virtual void Stop()
    {

    }
}

class LevelStateIntro : LevelState
{
    public GameObject playerFollowCamera;
    public CinemachineBrain cinemachineBrain;

    public Transform introTransform;

    public float intoHeight;

    public IsTriggered isGrounded;


    public LevelStateIntro(LevelStateMachine stateMachine, LevelStateSharedData sharedData) : base(stateMachine, sharedData)
    {
    }

    public override void Start()
    {
        stateMachine.StartCoroutine(StartIntro());
    }

    private IEnumerator StartIntro()
    {

        playerFollowCamera.SetActive(true);


        float elapsed = 0f;
        float maxDuration = 5.0f;


        characterMovement.Lock();
        attack.Lock();
        enemyAI.Lock();


        Vector3 toPosition = introTransform.position + new Vector3(0.0f, intoHeight, 0.0f);
        playerTransform.position = toPosition;


        for (int i = 0; i < trs.Length; i++)
        {
            trs[i].Clear();
        }


        while (elapsed < maxDuration &&
               !(isGrounded.isTriggered && elapsed > 0.2))
        {
            elapsed += Time.deltaTime;

            yield return null;
        }

        cinemachineCommonImpulseSource.GenerateImpulse(shakeCommonImpulseAmplitude);
        cinemachineDirectImpulseSource.GenerateImpulse(Vector3.down * shakeDirectImpulseAmplitude);


        enemyAI.Unlock();

        elapsed = 0f;
        maxDuration = 0.7f;

        while (elapsed < maxDuration)
        {
            elapsed += Time.deltaTime;

            yield return null;
        }


        //cinemachineBrain.enabled = false;
        cinemachineBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseIn;
        cinemachineBrain.m_DefaultBlend.m_Time = 1.0f;
        //cinemachineBrain.enabled = true;

        playerFollowCamera.SetActive(false);


        elapsed = 0f;
        maxDuration = 1.5f;

        while (elapsed < maxDuration)
        {
            elapsed += Time.deltaTime;

            yield return null;
        }

        // characterMovement.Unlock();
        // enemyAI.StartBattle();

        if (sharedData.isQuickGuideShown)
        {
            stateMachine.ChangeState(LevelStateType.Play);
        }
        else
        {
            sharedData.isQuickGuideShown = true;
            stateMachine.ChangeState(LevelStateType.QuickGuide);
        }


        // yield return null;

        // stateMachine.ChangeState(LevelStateType.QuickGuide);
    }
}

class LevelStateQuickGuide : LevelState
{

    public CanvasGroup canvasGroup;

    public float guideAppearingDuration;
    public float guideDisappearingDuration;


    public LevelStateQuickGuide(LevelStateMachine stateMachine, LevelStateSharedData sharedData) : base(stateMachine, sharedData)
    {
    }

    public override void Start()
    {
        stateMachine.StartCoroutine(StartQuickGuide());
    }

    private IEnumerator StartQuickGuide()
    {
        float elapsed = 0f;
        float duration = guideAppearingDuration;

        // cinemachineDirectImpulseSource.GenerateImpulse(Vector3.back * shakeDirectImpulseAmplitude);

        while (elapsed < duration)
        {
            float x = elapsed / duration;
            x = Utilities.EasyIn(x);

            canvasGroup.alpha = x;

            elapsed += Time.deltaTime;

            yield return null;
        }

        inputActions.Level.AnyButton.performed += OnAnyButtonPressed;


        canvasGroup.alpha = 1.0f;

        // stateMachine.ChangeState(LevelStateType.Play);
    }

    private IEnumerator EndQuickGuide()
    {
        float elapsed = 0f;
        float duration = guideDisappearingDuration;

        // cinemachineDirectImpulseSource.GenerateImpulse(Vector3.back * shakeDirectImpulseAmplitude);

        while (elapsed < duration)
        {
            float x = elapsed / duration;
            x = Utilities.EasyOut(x);

            canvasGroup.alpha = 1.0f - x;

            elapsed += Time.deltaTime;

            yield return null;
        }

        canvasGroup.alpha = 0.0f;

        stateMachine.ChangeState(LevelStateType.Play);
    }

    private void OnAnyButtonPressed(InputAction.CallbackContext _)
    {
        inputActions.Level.AnyButton.performed -= OnAnyButtonPressed;

        stateMachine.StartCoroutine(EndQuickGuide());
    }
}

class LevelStatePlay : LevelState
{
    public LevelStatePlay(LevelStateMachine stateMachine, LevelStateSharedData sharedData) : base(stateMachine, sharedData)
    {
    }

    public override void Start()
    {
        stateMachine.StartCoroutine(StartPlay());
    }

    private IEnumerator StartPlay()
    {
        characterMovement.Unlock();
        attack.Unlock();
        enemyAI.StartBattle();


        float elapsed = 0f;
        float maxDuration = 1.0f;


        yield return null;

        // while (true)
        // {
        //     elapsed += Time.deltaTime;

        //     yield return null;
        // }

        // stateMachine.ChangeState(LevelStateType.Play);
    }
}

class LevelStateUserDeath : LevelState
{
    public GameObject playerFollowCamera;
    public CinemachineBrain cinemachineBrain;
    public Transform introTransform;
    public float intoHeight;

    public LevelStateUserDeath(LevelStateMachine stateMachine, LevelStateSharedData sharedData) : base(stateMachine, sharedData)
    {
    }

    public override void Start()
    {

        Debug.Log("HAhahaha");

        stateMachine.StartCoroutine(StartUserDeath());

        //    stateMachine.StartCoroutine(StartTransitionToIntro());
        // OnRestartButtonPressed(new InputAction.CallbackContext());
    }

    private IEnumerator StartUserDeath()
    {
        characterMovement.Lock();
        attack.Lock();
        enemyAI.Lock();


        //cinemachineBrain.enabled = false;
        cinemachineBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseOut;
        cinemachineBrain.m_DefaultBlend.m_Time = 3.0f;
        //cinemachineBrain.enabled = true;

        playerFollowCamera.SetActive(true);

        yield return new WaitForSeconds(3.0f);

        enemyAI.StartStandingOverPlayersCorpse();

        yield return new WaitForSeconds(1.2f);


        gameOverUIObj.SetActive(true);
        gameOverUI.SetFloat("_Opacity", 0.0f);

        Transform gameOverTransform = gameOverUIObj.transform;


        Vector3 toPosition = gameOverTransform.position;
        Vector3 fromPosition = gameOverTransform.position + new Vector3(3.0f, 0.0f, 0.0f);




        float elapsed = 0f;
        float duration = 0.2f;

        while (elapsed < duration)
        {
            float x = elapsed / duration;
            x = Utilities.EasyIn(x);

            gameOverTransform.position = Vector3.Lerp(fromPosition, toPosition, x);

            gameOverUI.SetFloat("_Opacity", x);

            elapsed += Time.deltaTime;

            yield return null;
        }

        gameOverTransform.position = toPosition;

        cinemachineDirectImpulseSource.GenerateImpulse(Vector3.left * shakeDirectImpulseAmplitude * 0.5f);



        restartUIObj.SetActive(true);
        restartUI.SetFloat("_Opacity", 0.0f);

        Transform restartTransform = restartUIObj.transform;


        toPosition = restartTransform.position;
        fromPosition = restartTransform.position + new Vector3(-3.0f, 0.0f, 0.0f);

        elapsed = 0f;
        duration = 0.2f;

        while (elapsed < duration)
        {
            float x = elapsed / duration;
            x = Utilities.EasyIn(x);

            restartTransform.position = Vector3.Lerp(fromPosition, toPosition, x);

            restartUI.SetFloat("_Opacity", x);

            elapsed += Time.deltaTime;

            yield return null;
        }

        restartTransform.position = toPosition;

        cinemachineDirectImpulseSource.GenerateImpulse(Vector3.right * shakeDirectImpulseAmplitude * 0.5f);

        inputActions.Level.Restart.performed += OnRestartButtonPressed;
    }


    private void OnRestartButtonPressed(InputAction.CallbackContext _)
    {
        inputActions.Level.Restart.performed -= OnRestartButtonPressed;

        Debug.Log("Aaaaaa");

        gameOverUIObj.SetActive(false);
        youWinUIObj.SetActive(false);
        restartUIObj.SetActive(false);

        movement3D.ResetRotation();
        // playerTransform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);


        enemyTransform.position = introTransform.position + new Vector3(-intoHeight, 0.0f, 0.0f);

        playerHealth.Revive();
        enemyHealth.Revive();


        stateMachine.StartCoroutine(StartTransitionToIntro());

    }


    private IEnumerator StartTransitionToIntro()
    {
        Vector3 toPosition = introTransform.position + new Vector3(0.0f, intoHeight, 0.0f);

        float elapsed = 0f;
        float duration = 1.5f;

        float dampingFrom = 2.0f;
        float dampingTo = 0.2f;


        CinemachineTransposer cinemachineTransposer = playerFollowCamera.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>();

        cinemachineTransposer.m_YDamping = dampingFrom;

        yield return null;

        playerTransform.position = toPosition;
        playerTransform.GetComponent<Rigidbody>().velocity = Vector3.zero;

        for (int i = 0; i < trs.Length; i++)
        {
            trs[i].Clear();
        }

        Debug.Log("cinemachineTransposer " + cinemachineTransposer.m_YDamping);

        while (elapsed < duration)
        {
            float x = elapsed / duration;
            x = Utilities.EasyIn(x);

            cinemachineTransposer.m_YDamping = Mathf.Lerp(dampingFrom, dampingTo, x);

            playerTransform.position = toPosition;
            playerTransform.GetComponent<Rigidbody>().velocity = Vector3.zero;

            elapsed += Time.deltaTime;

            yield return null;
        }

        cinemachineTransposer.m_YDamping = dampingTo;

        stateMachine.ChangeState(LevelStateType.Intro);
    }

}


class LevelStateEnemyDeath : LevelState
{
    public Transform deathPosition;

    public GameObject playerFollowCamera;
    public CinemachineBrain cinemachineBrain;
    public Transform introTransform;
    public float intoHeight;


    public LevelStateEnemyDeath(LevelStateMachine stateMachine, LevelStateSharedData sharedData) : base(stateMachine, sharedData)
    {
    }

    public override void Start()
    {
        stateMachine.StartCoroutine(StartEnemyDeath());
    }

    private IEnumerator StartEnemyDeath()
    {
        characterMovement.Lock();
        attack.Lock();
        enemyAI.Lock();


        enemyAI.OnDeath();


        yield return new WaitForSeconds(1.3f);

        cinemachineBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseOut;
        cinemachineBrain.m_DefaultBlend.m_Time = 2.0f;

        playerFollowCamera.SetActive(true);


        float elapsed = 0f;
        float duration = 2.0f;


        Vector3 toPosition = deathPosition.position + new Vector3(0.0f, 0.0f, 0.0f);
        Vector3 fromPosition = playerTransform.position;

        while (elapsed < duration)
        {
            float x = elapsed / duration;
            x = Utilities.EasyOut(x);

            playerTransform.position = Vector3.Lerp(fromPosition, toPosition, x);

            elapsed += Time.deltaTime;

            yield return null;
        }


        // yield return new WaitForSeconds(3.0f);


        yield return new WaitForSeconds(2.0f);


        youWinUIObj.SetActive(true);
        youWinUI.SetFloat("_Opacity", 0.0f);

        Transform youWinTransform = youWinUIObj.transform;


        toPosition = youWinTransform.position;
        fromPosition = youWinTransform.position + new Vector3(3.0f, 0.0f, 0.0f);




        elapsed = 0f;
        duration = 0.2f;

        while (elapsed < duration)
        {
            float x = elapsed / duration;
            x = Utilities.EasyIn(x);

            youWinTransform.position = Vector3.Lerp(fromPosition, toPosition, x);

            youWinUI.SetFloat("_Opacity", x);

            elapsed += Time.deltaTime;

            yield return null;
        }

        youWinTransform.position = toPosition;

        cinemachineDirectImpulseSource.GenerateImpulse(Vector3.left * shakeDirectImpulseAmplitude * 0.5f);



        restartUIObj.SetActive(true);
        restartUI.SetFloat("_Opacity", 0.0f);

        Transform restartTransform = restartUIObj.transform;


        toPosition = restartTransform.position;
        fromPosition = restartTransform.position + new Vector3(-3.0f, 0.0f, 0.0f);

        elapsed = 0f;
        duration = 0.2f;

        while (elapsed < duration)
        {
            float x = elapsed / duration;
            x = Utilities.EasyIn(x);

            restartTransform.position = Vector3.Lerp(fromPosition, toPosition, x);

            restartUI.SetFloat("_Opacity", x);

            elapsed += Time.deltaTime;

            yield return null;
        }

        restartTransform.position = toPosition;

        cinemachineDirectImpulseSource.GenerateImpulse(Vector3.right * shakeDirectImpulseAmplitude * 0.5f);

        inputActions.Level.Restart.performed += OnRestartButtonPressed;
    }



    private void OnRestartButtonPressed(InputAction.CallbackContext _)
    {
        inputActions.Level.Restart.performed -= OnRestartButtonPressed;

        Debug.Log("Aaaaaa");

        gameOverUIObj.SetActive(false);
        youWinUIObj.SetActive(false);
        restartUIObj.SetActive(false);

        movement3D.ResetRotation();
        // playerTransform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);


        enemyTransform.position = introTransform.position + new Vector3(-intoHeight, 0.0f, 0.0f);

        playerHealth.Revive();
        enemyHealth.Revive();


        stateMachine.StartCoroutine(StartTransitionToIntro());

    }


    private IEnumerator StartTransitionToIntro()
    {
        Vector3 toPosition = introTransform.position + new Vector3(0.0f, intoHeight, 0.0f);

        float elapsed = 0f;
        float duration = 1.5f;

        float dampingFrom = 2.0f;
        float dampingTo = 0.2f;


        CinemachineTransposer cinemachineTransposer = playerFollowCamera.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>();

        cinemachineTransposer.m_YDamping = dampingFrom;

        yield return null;

        playerTransform.position = toPosition;
        playerTransform.GetComponent<Rigidbody>().velocity = Vector3.zero;

        for (int i = 0; i < trs.Length; i++)
        {
            trs[i].Clear();
        }

        Debug.Log("cinemachineTransposer " + cinemachineTransposer.m_YDamping);

        while (elapsed < duration)
        {
            float x = elapsed / duration;
            x = Utilities.EasyIn(x);

            cinemachineTransposer.m_YDamping = Mathf.Lerp(dampingFrom, dampingTo, x);

            playerTransform.position = toPosition;
            playerTransform.GetComponent<Rigidbody>().velocity = Vector3.zero;

            elapsed += Time.deltaTime;

            yield return null;
        }

        cinemachineTransposer.m_YDamping = dampingTo;

        stateMachine.ChangeState(LevelStateType.Intro);
    }

}