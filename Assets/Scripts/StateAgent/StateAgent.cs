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
    public IntRef health;
    public FloatRef timer;

    public GameObject enemy { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        stateMachine.AddState(new IdleState(this, "idle"));
        stateMachine.AddState(new PatrolState(this, "patrol"));
        stateMachine.AddState(new ChaseState(this, "chase"));

        stateMachine.AddTransition("idle",   new BoolTransition(enemySeen, true),  "chase");
        stateMachine.AddTransition("idle",   new FloatTransition(timer, Transition.Predicate.LESS, 0), "patrol");
        stateMachine.AddTransition("patrol", new BoolTransition(enemySeen, true),  "chase");
        stateMachine.AddTransition("chase",  new BoolTransition(enemySeen, false), "idle");

        stateMachine.setState(stateMachine.StateFromName("idle"));
    }

    // Update is called once per frame
    void Update()
    {
        //update parameters
        var gameObjects = perception.GetGameObjects();
        enemySeen.value = (gameObjects.Length != 0);
        enemy = (gameObjects.Length != 0) ? gameObjects[0] : null;

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
