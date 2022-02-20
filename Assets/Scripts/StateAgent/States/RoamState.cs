using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoamState : State
{
    public RoamState(StateAgent owner, string name) : base(owner, name)
    {

    }

    public override void OnEnter()
    {
        //Page 9 of assignment

        //< create a quaternion with a random angle between - 90 and 90 and rotate about the y axis>;
        Quaternion rotation = Quaternion.Euler(0, Random.Range(-90, 90), 0);

        //< set the forward vector by rotating the owner transform forward with the quaternion rotation >;
        Vector3 forward = rotation * owner.transform.position;

        //< position of the owner + forward + random float between 10 and 15 >;
        Vector3 destination = owner.transform.position + forward /*Random.Range(10, 15f)*/;

        owner.movement.MoveTowards(destination);
        owner.movement.Resume();
        owner.atDestination.value = false;
    }


    public override void OnExit()
    {
    }

    public override void OnUpdate()
    {
        //(<get distance between owner position and movement destination>
        if (Vector3.Distance(owner.transform.position, owner.movement.destination) <= 1.5)
	    {
            owner.atDestination.value = true;
        }
    }
}
