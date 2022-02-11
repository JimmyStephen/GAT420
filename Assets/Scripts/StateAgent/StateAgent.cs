using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateAgent : Agent
{
    [SerializeField] public Perception perception;
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
        stateMachine.AddState(new EvadeState(this, typeof(EvadeState).Name));

        //to patrol
        stateMachine.AddTransition(typeof(IdleState).Name,   new Transition ( new Condition[] { new FloatCondition(timer, Condition.Predicate.LESS_EQUAL, 0) }), typeof(PatrolState).Name);

        //to chase
        stateMachine.AddTransition(typeof(IdleState).Name,   new Transition ( new Condition[] { new BoolCondition(enemySeen, true) } ), typeof(ChaseState).Name);
        stateMachine.AddTransition(typeof(PatrolState).Name, new Transition ( new Condition[] { new BoolCondition(enemySeen, true) } ), typeof(ChaseState).Name);

        //to idle
        stateMachine.AddTransition(typeof(ChaseState).Name,  new Transition ( new Condition[] { new BoolCondition(enemySeen, false) } ), typeof(IdleState).Name);
        stateMachine.AddTransition(typeof(EvadeState).Name,  new Transition ( new Condition[] { new BoolCondition(enemySeen, false) } ), typeof(IdleState).Name);
        stateMachine.AddTransition(typeof(AttackState).Name, new Transition ( new Condition[] { new FloatCondition(timer, Condition.Predicate.LESS_EQUAL, 0) } ), typeof(IdleState).Name);

        //to evade
        //stateMachine.AddTransition(typeof(IdleState).Name,   new FloatTransition(health, Transition.Predicate.LESS_EQUAL, 30), typeof(EvadeState).Name);
        stateMachine.AddTransition(typeof(ChaseState).Name,  new Transition ( new Condition[] { new FloatCondition(health, Condition.Predicate.LESS_EQUAL, 30) } ), typeof(EvadeState).Name);
        stateMachine.AddTransition(typeof(AttackState).Name, new Transition ( new Condition[] { new FloatCondition(health, Condition.Predicate.LESS_EQUAL, 30) } ), typeof(EvadeState).Name);

        //to death
        stateMachine.AddTransition(typeof(IdleState).Name,   new Transition( new Condition[] { new FloatCondition(health, Condition.Predicate.LESS_EQUAL, 0) } ), typeof(DeathState).Name);
        stateMachine.AddTransition(typeof(ChaseState).Name,  new Transition( new Condition[] { new FloatCondition(health, Condition.Predicate.LESS_EQUAL, 0) } ), typeof(DeathState).Name);
        stateMachine.AddTransition(typeof(PatrolState).Name, new Transition( new Condition[] { new FloatCondition(health, Condition.Predicate.LESS_EQUAL, 0) } ), typeof(DeathState).Name);

        //to Attack State
        stateMachine.AddTransition(typeof(ChaseState).Name,  new Transition ( new Condition[] { new FloatCondition(enemyDistance, Condition.Predicate.LESS_EQUAL, 2) } ), typeof(AttackState).Name);
        
        stateMachine.setState(stateMachine.StateFromName(typeof(IdleState).Name));
    }

    // Update is called once per frame
    void Update()
    {
        timer.value -= Time.deltaTime;
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
