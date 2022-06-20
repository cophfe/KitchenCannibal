using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Order : MonoBehaviour
{
    public float startTime;
    public float timeTillFail;
    public Recipe recipe;
    public int reward;
    private bool timeStarted = false;
    public void StartTime()
    {
        timeStarted = true;
    }

    private void Update()
    {
        if(timeStarted)
        {
            timeTillFail -= Time.deltaTime;

            if(timeTillFail <= 0.0f)
            OrderFailed();
        }
    }

    public void OrderFailed()
    {
        // Run stuff here when order fails
        timeStarted = false;
        Debug.Log(recipe.name + "Order failed");
        
    }
    
    public void OrderComplete()
    {
        timeStarted = false;
        Debug.Log(recipe.name + " Order complete");
    }
}
