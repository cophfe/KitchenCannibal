using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OrderDisplay : MonoBehaviour
{
    [SerializeField] private Image orderImage = null;
    [SerializeField] private Image[] orderIngredientImages = null;
    [SerializeField] TMP_Text[] orderIngredientAmount = null;
    [SerializeField] private TMP_Text orderDescription = null;
    

    /// <summary>
    /// Called whena new order has been made,
    /// Updates the display once,
    /// </summary>
    public void UpdateDisplay(Order order)
    {
        if(order != null)
        { 
            for(int i = 0; i <order.recipe.recipeRequirements.Count; i++)
            {
                if(i < 4)
                {
                    orderIngredientAmount[i].text = order.recipe.recipeRequirements[i].amount.ToString();

                    switch (order.recipe.recipeRequirements[i].ingredient)
                    {
                        case IngredientType.cookedMeat:
                             orderIngredientImages[i].sprite = GameManager.Instance.modelsAndimages.cookedMeat;
                            break;
                        case IngredientType.buns:
                             orderIngredientImages[i].sprite = GameManager.Instance.modelsAndimages.buns;
                            break;
                        case IngredientType.lettuce:
                             orderIngredientImages[i].sprite = GameManager.Instance.modelsAndimages.lettuce;
                            break;
                        case IngredientType.tomatoe:
                             orderIngredientImages[i].sprite = GameManager.Instance.modelsAndimages.tomatoe;
                            break;
                    }
                }

                switch (order.recipe.completedRecipie)
                {
                    case CompletedRecipieType.BLT:
                        orderImage.sprite = orderIngredientImages[i].sprite = GameManager.Instance.modelsAndimages.BLT;
                        break;
                }

                orderDescription.text = order.recipe.description;
            }
        }
    }

    /// <summary>
    /// Called every frame,
    /// Updates the timer on the order,
    /// </summary>
    public void UpdateTime(float percent)
    {
        // Update the timer display
    }
}
