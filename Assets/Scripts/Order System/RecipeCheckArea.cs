using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeCheckArea : MonoBehaviour
{
    public List<RecipeRequirement> ingredientsPresent;
    [SerializeField] OrderManager orderManager = null;
    public List<GameObject> objects;
    [SerializeField] private GameObject foodSpawnLocation = null;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger: " + other.name);

        Ingredient newIngredient = other.gameObject.GetComponent<Ingredient>();
        if(newIngredient != null)
        {
            objects.Add(other.gameObject);
            bool ingredientFound = false;

            // Check if ingredient already exists and add a count of one to it
            for(int i = 0; i < ingredientsPresent.Count; i++)
            {
                if(ingredientsPresent[i].ingredient == newIngredient.ingredientType)
                {
                    ingredientsPresent[i].amount++;
                    ingredientFound = true;
                    break;
                }
            }

            if(!ingredientFound)
            {
                RecipeRequirement temp = new RecipeRequirement();
                temp.amount = 1;
                temp.ingredient = newIngredient.ingredientType;
                ingredientsPresent.Add(temp);
            }
        }

        else if(other.tag == "Check")
        {
            orderManager.CheckRecipe(ingredientsPresent, foodSpawnLocation.transform.position);
            ingredientsPresent.Clear();
            for(int i = 0;i < objects.Count;i++)
            {
                Destroy(objects[i]);
            }
            objects.Clear();
        }
    }
}
