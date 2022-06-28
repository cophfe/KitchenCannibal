using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Order : MonoBehaviour
{
    [HideInInspector] public OrderDisplay display;
    public float startTime;
    public float timeTillFail;
    public Recipe recipe;
    public int reward;
    private bool timeStarted = false;
    [HideInInspector] public int orderRackIndex = 0;
    [HideInInspector] public OrderRack rack = null;
    public bool orderActive = false;
    public void StartTime()
    {
        orderActive = true;
        timeStarted = true;
    }

    private void Update()
    {
        if(timeStarted)
        {
            timeTillFail -= Time.deltaTime;
            //display.UpdateTime(timeTillFail);

            if(timeTillFail <= 0.0f)
            OrderFailed();
        }
    }

    public void OrderFailed()
    {
        // Run stuff here when order fails
        timeStarted = false;
        Debug.Log(recipe.name + "Order failed");
        GameManager.Instance.audioManager.PlayOneShot(SoundSources.Order, 0);
        rack.RemoveOrder(orderRackIndex);
        Destroy(display.gameObject);
    }

    public void CreateOrder(Vector3 position)
    {
        GameObject temp = Instantiate(GameManager.Instance.modelsAndimages.burgerPrefab);
        Order tempOrder = temp.GetComponent<Order>();
        tempOrder.display = display;
        tempOrder.recipe = recipe;
        tempOrder.reward = reward;
        tempOrder.orderRackIndex = orderRackIndex;
        tempOrder.rack = rack;
        tempOrder.transform.position = position;
    }

    public void OrderComplete()
    {        
        timeStarted = false;
        Debug.Log(recipe.name + " Order complete");
        rack.RemoveOrder(orderRackIndex);
        Destroy(display.gameObject);
        Debug.Log("Order complete! " + reward + " Points awarded");
        GameManager.Instance.audioManager.PlayOneShot(SoundSources.Order, 1);
    }
}
