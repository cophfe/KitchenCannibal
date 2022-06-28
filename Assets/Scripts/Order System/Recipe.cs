using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RecipeRequirement
{
    public IngredientType ingredient;
    public int amount;
}

[CreateAssetMenu(fileName = "Recipe", menuName = "New Recipe")]
public class Recipe : ScriptableObject
{
    public string name;
    public string description;

    public CompletedRecipieType completedRecipie;
    public List<RecipeRequirement> recipeRequirements;
}
