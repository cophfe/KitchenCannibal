using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RecipeRequirement
{
    public IngredientType ingredient;
    public float amount;
}

[CreateAssetMenu(fileName = "Recipe", menuName = "New Recipe")]
public class Recipe : ScriptableObject
{
    new public string name;
    public string description;

    public CompletedRecipieType completedRecipie;
    public List<RecipeRequirement> recipeRequirements;
}
