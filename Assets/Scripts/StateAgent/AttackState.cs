using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State
{
    public AttackState(StateAgent owner, string name) : base(owner, name)
    {

    }

    public override void OnEnter()
    {
        owner.movement.Stop();
        owner.animator.SetTrigger("Attack");
        owner.timer.value = 1;
    }

    public override void OnExit()
    {
        //throw new System.NotImplementedException();
    }

    public override void OnUpdate()
    {
    }
}
