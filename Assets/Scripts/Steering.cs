using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Steering : MonoBehaviour
{

    [SerializeField] float wanderDistance = 1;
    [SerializeField] float wanderRadius = 3;
    [SerializeField] float wanderDisplacement = 5;
    float wanderAngle = 0;
        
    public Vector3 Seek(AutonomousAgent agent, GameObject target)
    {
        Vector3 force = CalculateSteering(agent, target.transform.position - agent.transform.position);
        return force;
    }
    public Vector3 Flee(AutonomousAgent agent, GameObject target)
    {
        Vector3 force = CalculateSteering(agent, agent.transform.position - target.transform.position);
        return force;
    }

    public Vector3 Wander(AutonomousAgent agent)
    {
        wanderAngle = wanderAngle + Random.Range(-wanderDisplacement, wanderDisplacement);
             
        Quaternion rotation = Quaternion.AngleAxis(wanderAngle, Vector3.up);
        Vector3 point = rotation * (Vector3.forward * wanderRadius); 
        Vector3 forward = agent.transform.forward * wanderDistance;
        Vector3 force = CalculateSteering(agent, forward + point);

        return force;
    }

    Vector3 CalculateSteering(AutonomousAgent agent, Vector3 direction)
    {
        Vector3 desired = direction.normalized * agent.maxSpeed;
        Vector3 steer = desired - agent.velocity;
        Vector3 force = Vector3.ClampMagnitude(steer, agent.maxForce);
        return force;
    }
}
