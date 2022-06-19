using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum IngredientType
{
    empty = 1,
    humanMeat = 2,
    mincedHumanMeat = 3,
    cookedMeat = 4,
    buns = 5,
}

public class OrderManager : MonoBehaviour
{
    public List<Order> orders = null;
    private int currentOrderIndex = 0;

    public float elaspedTime = 0.0f;

    // Update is called once per frame
    void Update()
    {
        if (currentOrderIndex < orders.Count)
        {
            if (elaspedTime >= orders[currentOrderIndex].startTime)
            {
                Debug.Log("Order: " + currentOrderIndex + " has started!");
                // Create orders here
                orders[currentOrderIndex].StartTime();
                currentOrderIndex++;
            }
        }

        elaspedTime += Time.deltaTime;
    }

    public bool CheckRecipe(List<RecipeRequirement> recipe)
    {

        for (int i = 0; i < orders.Count; i++)
        {
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
                        break;
                    }
                }

                // An ingredient type did not match
                if (correctIngredients != x)
                    break;
                else
                {
                    if (recipe[correctIndex].amount != orders[i].recipe.recipeRequirements[x].amount)
                        break;
                }
            }

            // A match is found
            orders[i].OrderComplete();
            return true;
        }

        // Nothing is found
        return false;
    }
}
