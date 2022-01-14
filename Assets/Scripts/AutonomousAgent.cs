using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutonomousAgent : Agent
{
    [SerializeField] Perception perception;
    [SerializeField] Perception flockPerception;
    [SerializeField] Steering steering;
    [SerializeField] AutonomousAgentData agentData;

    public float maxSpeed { get { return agentData.maxSpeed; } }
    public float maxForce { get { return agentData.maxForce; } }
    
    public Vector3 velocity { get; set; } = Vector3.zero;


    void Update()
    {
        Vector3 acceleration = Vector3.zero;


        GameObject[] gameObjects = perception.GetGameObjects();
        //wander
        if (gameObjects.Length == 0)
        {
            acceleration += steering.Wander(this);
        }
        //seek / flee (player)
        if (gameObjects.Length != 0)
        {
            acceleration += steering.Seek(this, gameObjects[0]) * agentData.seekWeight;
            acceleration += steering.Flee(this, gameObjects[0]) * agentData.fleeWeight;
        }
        //flocking
        gameObjects = flockPerception.GetGameObjects();
        if(gameObjects.Length != 0)
        {
            //collect
            acceleration += steering.Cohesion(this, gameObjects) * agentData.cohesionWeight;
            //seperate
            acceleration += steering.Seperation(this, gameObjects, agentData.separationRadius) * agentData.separationWeight;
            //align
            acceleration += steering.Alignment(this, gameObjects) * agentData.alignmentWeight;
        }

        velocity += acceleration * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
        transform.position += velocity * Time.deltaTime;

        if (velocity.sqrMagnitude == 0.1f)
        {
            transform.rotation = Quaternion.LookRotation(velocity);
        }
    }
}
