using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateAgent : Agent
{
    [SerializeField] Perception perception;
    public StateMachine stateMachine = new StateMachine();
    //    [HideInInspector] can be used to hide public variables from inspecter
    public PathFollower pathFollower;

    public BoolRef enemySeen;
    public FloatRef health;
    public FloatRef enemyDistance;
    public FloatRef timer;

    public GameObject enemy { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        stateMachine.AddState(new IdleState(this, typeof(IdleState).Name));
        stateMachine.AddState(new PatrolState(this, typeof(PatrolState).Name));
        stateMachine.AddState(new ChaseState(this, typeof(ChaseState).Name));
        stateMachine.AddState(new DeathState(this, typeof(DeathState).Name));
        stateMachine.AddState(new AttackState(this, typeof(AttackState).Name));

        //to patrol
        stateMachine.AddTransition(typeof(IdleState).Name,   new FloatTransition(timer, Transition.Predicate.LESS_EQUAL, 0), typeof(PatrolState).Name);

        //to chase
        stateMachine.AddTransition(typeof(IdleState).Name,   new BoolTransition(enemySeen, true), typeof(ChaseState).Name);
        stateMachine.AddTransition(typeof(PatrolState).Name, new BoolTransition(enemySeen, true), typeof(ChaseState).Name);

        //to idle
        stateMachine.AddTransition(typeof(ChaseState).Name,  new BoolTransition(enemySeen, false), typeof(IdleState).Name);
        stateMachine.AddTransition(typeof(AttackState).Name, new FloatTransition(timer, Transition.Predicate.LESS_EQUAL, 0), typeof(IdleState).Name);

        //to death
        stateMachine.AddTransition(typeof(IdleState).Name,   new FloatTransition(health, Transition.Predicate.LESS_EQUAL, 0), typeof(DeathState).Name);
        stateMachine.AddTransition(typeof(ChaseState).Name,  new FloatTransition(health, Transition.Predicate.LESS_EQUAL, 0), typeof(DeathState).Name);
        stateMachine.AddTransition(typeof(PatrolState).Name, new FloatTransition(health, Transition.Predicate.LESS_EQUAL, 0), typeof(DeathState).Name);

        //Attack State
        stateMachine.AddTransition(typeof(ChaseState).Name,  new FloatTransition(health, Transition.Predicate.LESS_EQUAL, 1), typeof(AttackState).Name);
        
        stateMachine.setState(stateMachine.StateFromName(typeof(IdleState).Name));
    }

    // Update is called once per frame
    void Update()
    {
        //update parameters
        var enemies = perception.GetGameObjects();
        enemySeen.value = (enemies.Length != 0);
        enemy = (enemies.Length != 0) ? enemies[0] : null;
        enemyDistance.value = (enemy != null) ? (Vector3.Distance(transform.position, enemy.transform.position)) : float.MaxValue;

        stateMachine.Update();

        animator.SetFloat("Speed", movement.velocity.magnitude);
        //The old way
/*        if(movement.velocity.magnitude > 0.5)
        {
            animator.SetBool("Walk", true);
        }
        else
        {
            animator.SetBool("Walk", false);
        }*/
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10,10,300,20), stateMachine.GetStateName());
    }
}
