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
    private bool timeStarted = false;
    [HideInInspector] public int orderRackIndex = 0;
    [HideInInspector] public OrderRack rack = null;
    public bool orderActive = false;
    private float startingTime = 0.0f;
	public bool hasBones = false;

    public void StartTime()
    {
        orderActive = true;
        timeStarted = true;
		GameManager.Instance.RegisterOrder();
	}

	private void Update()
    {
        if (timeStarted && orderActive)
        {
            timeTillFail -= Time.deltaTime;
            if (display != null)
				display.UpdateTime(timeTillFail / startingTime);

            if (timeTillFail <= 0.0f)
                OrderFailed();
        }
    }

    public void OrderFailed()
    {
        // Run stuff here when order fails
        timeStarted = false;
		orderActive = false;

		Debug.Log(recipe.name + "Order failed");
        GameManager.Instance.audioManager.PlayOneShot(SoundSources.Order, 0);
        GameManager.Instance.scoreKeeper.ChangeScore(ScoreChange.OrderFailed);
        rack.RemoveOrder(orderRackIndex);
        Destroy(display.gameObject);

		GameManager.Instance.DeregisterOrder();
	}

	public void CreateOrder(Transform spawnTransform, bool hasBones)
    {
		GameObject prefab;
		switch (recipe.completedRecipie)
		{
			case CompletedRecipieType.Burger:
				prefab = GameManager.Instance.modelsAndimages.burgerPrefab;
				break;
			case CompletedRecipieType.Salad:
				prefab = GameManager.Instance.modelsAndimages.saladPrefab;
				break;
			case CompletedRecipieType.HotDog:
				prefab = GameManager.Instance.modelsAndimages.hotdogPrefab;
				break;
			default:
				return;
		}
		GameObject temp = Instantiate(prefab);
        Order tempOrder = temp.GetComponent<Order>();
        tempOrder.display = display;
        tempOrder.recipe = recipe;
        tempOrder.orderRackIndex = orderRackIndex;
        tempOrder.rack = rack;
		tempOrder.hasBones = hasBones;
		tempOrder.transform.position = spawnTransform.position;
		tempOrder.transform.rotation = spawnTransform.rotation;
		tempOrder.timeStarted = false;
	}

	public void OrderComplete()
    {
        timeStarted = false;
		orderActive = false;

        if (display == null)
            return;

        Debug.Log(recipe.name + " Order complete");
        rack.RemoveOrder(orderRackIndex);
        Destroy(display.gameObject);

		Debug.Log("Order complete! Points awarded");
        GameManager.Instance.audioManager.PlayOneShot(SoundSources.Order, 1);
		if (hasBones)
		{
			GameManager.Instance.scoreKeeper.ChangeScore(ScoreChange.BonesOrder);
			GameManager.Instance.healthInspector.OnBonesMeal();
		}
		else
			GameManager.Instance.scoreKeeper.ChangeScore(ScoreChange.OrderComplete);

		GameManager.Instance.DeregisterOrder();
	}
	private void Awake()
    {
        startingTime = timeTillFail;
    }
}
