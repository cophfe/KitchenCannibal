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

    public bool CheckRecipe(List<RecipeRequirement> recipe, Vector3 spawnLocation)
    {
        bool completedOrder = true;
        for (int i = 0; i < currentOrderIndex; i++)
        {
            // Has searched through all active orders
            if (!orders[i].orderActive)
                continue;

            // The amount of ingredients present is not the same
            if (orders[i].recipe.recipeRequirements.Count != recipe.Count)
                continue;

            int correctIngredients = 0;
            int correctIndex = 0;
            for (int x = 0; x < orders[i].recipe.recipeRequirements.Count; x++)
            {
                for (int y = 0; y < recipe.Count; y++)
                {
                    // Ingredient is the same
                    if (orders[i].recipe.recipeRequirements[x].ingredient == recipe[y].ingredient)
                    {
                        correctIngredients++;
                        correctIndex = y;
                        break; // ingredient is the same
                    }
                }

                // An ingredient type did not match
                if (correctIngredients != x + 1)
                {
                    completedOrder = false;
                    break; // An ingredient from the order cannot be found checks other orders
                }

                else
                {
                    if (recipe[correctIndex].amount != orders[i].recipe.recipeRequirements[x].amount)
                    {
                        completedOrder = false;
                        break; // The amount of said ingredient needed is not present
                    }
                }
            }

            if (!completedOrder)
            {
                completedOrder = true;
                continue;
            }

            // A match is found
            orders[i].orderActive = false;
            orders[i].CreateOrder(spawnLocation);
            return true;
        }

        // Nothing is found
        Debug.Log("Invalid Recipe");
        return false;
    }
}
