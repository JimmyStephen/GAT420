using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateAgent : Agent
{
    public StateMachine stateMachine = new StateMachine();
    //    [HideInInspector] can be used to hide public variables from inspecter
    public AgentPath path;

    // Start is called before the first frame update
    void Start()
    {
        stateMachine.AddState(new IdleState(this, "idle"));
        stateMachine.AddState(new PatrolState(this, "patrol"));
        stateMachine.setState(stateMachine.StateFromName("idle"));
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();

        if(movement.velocity.magnitude > 0.5)
        {
            animator.SetBool("Walk", true);
        }
        else
        {
            animator.SetBool("Walk", false);
        }
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10,10,300,20), stateMachine.GetStateName());
    }
}
