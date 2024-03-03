using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform[] spawnPoints;




}







class StateMachine {

    private State currentState;

    public StateMachine() {
        currentState = new IntroState(this);
    }

    public void Update() {

    }

    
    public void ChangeState(State state) {

    }
}


enum StateType {
    Intro,
    GoToNewPosition,
    AttackPreparation,
    // We can cancel the attack if the player is fast enough
    Attack,
    Damage,
    Dead
}

class State {

    //static Dictionary<StateType, State> states =





    private StateMachine stateMachine;

    public State(StateMachine stateMachine) {
        this.stateMachine = stateMachine;
    }

    public void Update() {

    }

}


class IntroState : State {

    public IntroState(StateMachine stateMachine) : base(stateMachine) {

    }

    public void Update() {

    }
    
}


