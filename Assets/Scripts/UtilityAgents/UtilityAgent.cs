using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilityAgent : Agent
{
    [SerializeField] MeterUI meter;
    Need[] needs;

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
        meter.slider.value = 1 - happiness;
        meter.worldPosition = transform.position;
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
}
