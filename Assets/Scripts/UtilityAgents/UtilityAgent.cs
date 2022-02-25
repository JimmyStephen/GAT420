﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UtilityAgent : Agent
{
    [SerializeField] MeterUI meter;
    [SerializeField] Perception perception;

    const float MIN_SCORE = 0.1f;

    Need[] needs;
    UtilityObject activeUtilityObject = null;
    public bool isUsingUtilityObject { get { return activeUtilityObject != null; } }

    public float happiness
    {
        get
        {
            float total = 0;
            foreach(var need in needs)
            {
                total += need.motive;
            }
            return total / needs.Length;
        }
    }

    void Start()
    {
        needs = GetComponentsInChildren<Need>();
        meter.text.text = "";

    }

    private void Update()
    {
        animator.SetFloat("Speed", movement.velocity.magnitude);
        if(activeUtilityObject == null)
        {
            var gameObjects = perception.GetGameObjects();
            List<UtilityObject> utilityObjects = new List<UtilityObject>();
            foreach(var go in gameObjects)
            {
                if(go.TryGetComponent<UtilityObject>(out UtilityObject utilityObject))
                {
                    utilityObject.visible = true;
                    utilityObject.score = GetUtilityObjectScore(utilityObject);
                    if(utilityObject.score > MIN_SCORE) utilityObjects.Add(utilityObject);
                }
            }

            activeUtilityObject = GetHighestUtilityObject(utilityObjects.ToArray());
            if(activeUtilityObject != null)
            {
                StartCoroutine(ExecuteUtilityObject(activeUtilityObject));
            }
        }
    }

    private void OnGUI()
    {
        Vector2 screen = Camera.main.WorldToScreenPoint(transform.position);

        GUI.color = Color.black;
        int offset = 0;
        foreach (var need in needs)
        {
            GUI.Label(new Rect(screen.x + 20, Screen.height - screen.y - offset, 300, 20), need.type.ToString() + ": " + need.motive);
            offset += 20;
        }
        //GUI.Label(new Rect(screen.x + 20, Screen.height - screen.y - offset, 300, 20), mood.ToString());
    }

    private void LateUpdate()
    {
        meter.slider.value = 1 - happiness;
        meter.worldPosition = transform.position;
    }

    IEnumerator ExecuteUtilityObject(UtilityObject utilityObject)
    {
        //go to location
        movement.MoveTowards(utilityObject.location.position);
        while(Vector3.Distance(transform.position, utilityObject.location.position) > 0.5)
        {
            Debug.DrawLine(transform.position, utilityObject.location.position);
            yield return null;
        }

        //start effect
        print("Start Effect");
        if (utilityObject.effect != null) utilityObject.effect.SetActive(true);

        //wait duration
        yield return new WaitForSeconds(utilityObject.duration);

        //end effect
        print("End Effect");
        if (utilityObject.effect != null) utilityObject.effect.SetActive(false);

        //Apply
        applyUtilityObject(utilityObject);
        
        //set active to null
        activeUtilityObject = null;

        //exit
        yield return null;
    }

    void applyUtilityObject(UtilityObject utilityObject)
    {
        foreach (var effector in utilityObject.effectors)
        {
            Need need = GetNeedByType(effector.type);
            if (need != null)
            {
                need.input += effector.change;
                need.input = Mathf.Clamp(need.input, -1, 1);
            }
        }
    }

    float GetUtilityObjectScore(UtilityObject utilityObject)
    {
        float score = 0;

        foreach(var effector in utilityObject.effectors)
        {
            Need need = GetNeedByType(effector.type);
            if(need != null)
            {
                float futureNeed = need.getMotive(need.input + effector.change);
                score += need.motive - futureNeed;
            }
        }

        return score;
    }

    Need GetNeedByType(Need.Type type)
    {
        return needs.First(need => need.type == type);
    }

    UtilityObject GetHighestUtilityObject(UtilityObject[] utilityObjects)
    {
        UtilityObject highestUtilityObject = null;
        float highestScore = MIN_SCORE;
        foreach (var utilityObject in utilityObjects)
        {
            // get the score of the utility object
            float temp = utilityObject.score;
            // if score > highest score then set new highest score and highest utility object
            if(temp > highestScore)
            {
                highestUtilityObject = utilityObject;
            }
        }
        return highestUtilityObject;
    }

    UtilityObject GetRandomUtilityObject(UtilityObject[] utilityObjects)
    {
        // evaluate all utility objects
        float[] scores = new float[utilityObjects.Length];
        float totalScore = 0;
        for (int i = 0; i < utilityObjects.Length; i++)
        {
            // <get the score of the utility objects[i]>
            float score = utilityObjects[i].score;
            // <set the scores[i] to the score>
            scores[i] = score;
            // <add score to total score>
            totalScore += score;
        }

        // select random utility object based on score
        // the higher the score the greater the chance of being randomly selected
        float random = Random.Range(0, totalScore);
        for (int i = 0; i < scores.Length; i++)
        {
            // <check if random value is less than scores[i]>
            if(random < scores[i])
            {
                // <return utilityObjects[i] if less than>
                return utilityObjects[i];
            }
            random -= scores[i];
            // <subtract scores[i] from random value>
        }

        return null;
    }

}
