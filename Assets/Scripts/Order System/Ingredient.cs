using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingredient : MonoBehaviour
{
    [HideInInspector] public float processedAmount = 0.0f;
    public float timeToCook = 5.0f;
    public float ingredientAmount = 0.0f;
    public IngredientType ingredientType;

    public void Process()
    {
        // human meat  - > minced meat -> cooked meat
        switch (ingredientType)
        {
            case IngredientType.empty:
                break;

            case IngredientType.humanMeat:
                ingredientType = IngredientType.mincedHumanMeat;
                break;

            case IngredientType.mincedHumanMeat:
                ingredientType = IngredientType.cookedMeat;
                break;

            case IngredientType.cookedMeat:
                break;

            case IngredientType.buns:
                break;

        }
    }
}
