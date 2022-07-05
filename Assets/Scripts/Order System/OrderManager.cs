using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum IngredientType
{
    Lettuce,
    SlicedLettuce,
    Tomatoe,
    SlicedTomatoe,
    RawMeat,
    MincedMeat,
    CookedMeat,
    BurntMeat,
    Bread,
    SlicedBread
}

public enum CompletedRecipieType
{
    Burger,
    Salad,
    HotDog,
}

public class OrderManager : MonoBehaviour
{
    private Order[] orders = null;
    private int currentOrderIndex = 0;
    [SerializeField] private OrderRack rack = null;

    public float elaspedTime = 0.0f;

    private void Awake()
    {
        orders = GetComponents<Order>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentOrderIndex < orders.Length)
        {
            if (elaspedTime >= orders[currentOrderIndex].startTime)
            {
                Debug.Log("Order: " + currentOrderIndex + " has started!");
                // Create orders here
                rack.AddOrder(orders[currentOrderIndex]);
                currentOrderIndex++;
            }
        }

        elaspedTime += Time.deltaTime;
    }

    public bool CheckRecipe(List<Ingredient> recipe, Transform spawnTransform)
    {
        bool completedOrder = true;
		for (int i = 0; i < currentOrderIndex; i++)
		{
			// Has searched through all active orders
			if (!orders[i].orderActive)
				continue;

			//check if there is enough ingredient for each requirement
			int requirementsFufilled = 0;
			for (int j = 0; j < orders[i].recipe.recipeRequirements.Count; j++)
			{
				var requirement = orders[i].recipe.recipeRequirements[j];
				float amount = 0;
				for (int k = 0; k < recipe.Count; k++)
				{
					var ingredient = recipe[k];
					if (ingredient.ingredientType == requirement.ingredient)
					{

						amount += ingredient.ingredientAmount;
					}
				}

				if (amount > requirement.amount)
					requirementsFufilled++;
				else
					Debug.Log("not enough ingredient found: " + requirement.ingredient);
			}

			if (requirementsFufilled < orders[i].recipe.recipeRequirements.Count)
			{
				completedOrder = false;
			}

			//check their are no non matching ingredients
			foreach (var ingredient in recipe)
			{
				bool matchFound = false;
				foreach (var req in orders[i].recipe.recipeRequirements)
				{
					matchFound |= ingredient.ingredientType == req.ingredient;
				}
				if (!matchFound)
				{
					Debug.Log("incorrect ingredient found: " + ingredient.ingredientType);
					completedOrder = false;
					break;
				}
			}

			if (!completedOrder)
            {
                completedOrder = true;
                continue;
            }

            // A match is found
            orders[i].orderActive = false;
            orders[i].CreateOrder(spawnTransform);
            return true;
        }

        // Nothing is found
        Debug.Log("Invalid Recipe");
        return false;
    }
}
