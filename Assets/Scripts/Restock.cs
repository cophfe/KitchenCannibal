using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Restock : MonoBehaviour
{
	[SerializeField]
	GameObject foodPrefab;
	[SerializeField]
	Transform respawnPoint;
	[SerializeField]
	CheckFridge fridgeChecker;
	[SerializeField]
	bool forceFridgeClosed = true;
	[SerializeField]
	int destroyCallsPerFrame = 5;
	[SerializeField]
	float restockDelay = 1;
	[SerializeField]
	DoorClose[] doors = null;

	float restockTimer= 0;
	bool restocking;
	JointSpring closeSpring;
	JointSpring openSpring;
	List<Ingredient> ingredientsInFridge = new List<Ingredient>();

	public void StartRestockFridge()
	{
		restocking = true;
		restockTimer = restockDelay;

		
		if (forceFridgeClosed)
			foreach (var door in doors)
				door.Slam();
	}

	void FinishRestock()
	{
		restocking = false;

		//destroy any leftover ingredients
		if (ingredientsInFridge.Count > 0)
		{
			foreach (var ingredient in ingredientsInFridge)
			{
				if (ingredient)
					Destroy(ingredient.gameObject);
			}
		}

		fridgeChecker.GetAllIngredientsInBox(ingredientsInFridge);

		//disable all ingredients in fridge (and destroy them all overtime)
		foreach(var ingredient in ingredientsInFridge)
		{
			ingredient.gameObject.SetActive(false);
		}

		if (ingredientsInFridge.Count == 0)
		{
			//respawn (slow asf but harder to account for)
			var food = Instantiate(foodPrefab, respawnPoint);
		}
		
	}

	private void Update()
	{
		//destroy ingredients in fridge
		if (ingredientsInFridge.Count > 0)
		{
			for (int i = 0; i <  destroyCallsPerFrame && i < ingredientsInFridge.Count; i++)
			{
				if (ingredientsInFridge[i])
				{
					Destroy(ingredientsInFridge[i].gameObject);
				}
			}

			ingredientsInFridge.RemoveRange(0, Mathf.Min(destroyCallsPerFrame, ingredientsInFridge.Count));
			if (ingredientsInFridge.Count == 0)
			{
				//respawn (slow asf but harder to account for)
				var food = Instantiate(foodPrefab, respawnPoint);
			}
		}

		if (restocking)
		{
			restockTimer -= Time.deltaTime;
			if (restockTimer <= 0)
			{
				FinishRestock();
			}
		}
	}
}
