using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State
{
    float timer;

    public IdleState(StateAgent owner, string name) : base(owner, name)
    {

    }

    public override void OnEnter()
    {
        timer = 2;
    }

    public override void OnExit()
    {

    }

    public override void OnUpdate()
    {
        timer -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space) || timer <= 0)
        {
            owner.stateMachine.setState(owner.stateMachine.StateFromName("patrol"));
        }
    }
}
