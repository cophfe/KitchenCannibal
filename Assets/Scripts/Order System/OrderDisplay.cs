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
    [SerializeField] public Image amountLeft = null;


    /// <summary>
    /// Called whena new order has been made,
    /// Updates the display once,
    /// </summary>
    public void UpdateDisplay(Order order)
    {
        if (order != null)
        {
            for (int i = 0; i < order.recipe.recipeRequirements.Count && i < 4; i++)
            {
                orderIngredientAmount[i].text = order.recipe.recipeRequirements[i].amount.ToString();

                switch (order.recipe.recipeRequirements[i].ingredient)
                {
                    case IngredientType.CookedMeat:
                        orderIngredientImages[i].sprite = GameManager.Instance.modelsAndimages.cookedMeat;
                        break;
                    case IngredientType.SlicedBread:
                        orderIngredientImages[i].sprite = GameManager.Instance.modelsAndimages.buns;
                        break;
                    case IngredientType.SlicedLettuce:
                        orderIngredientImages[i].sprite = GameManager.Instance.modelsAndimages.lettuce;
                        break;
                    case IngredientType.SlicedTomatoe:
                        orderIngredientImages[i].sprite = GameManager.Instance.modelsAndimages.tomatoe;
                        break;
                }
            }
			for (int i = order.recipe.recipeRequirements.Count; i < 4; i++)
			{
				orderIngredientAmount[i].text = "";
				orderIngredientImages[i].enabled = false;
			}

            switch (order.recipe.completedRecipie)
            {
                case CompletedRecipieType.Burger:
                    orderImage.sprite = GameManager.Instance.modelsAndimages.burger;
                    break;

                case CompletedRecipieType.HotDog:
                    orderImage.sprite = GameManager.Instance.modelsAndimages.hotdog;
                    break;

                case CompletedRecipieType.Salad:
                    orderImage.sprite = GameManager.Instance.modelsAndimages.salad;
                    break;
            }

            orderDescription.text = order.recipe.description;
        }
    }

    /// <summary>
    /// Called every frame,
    /// Updates the timer on the order,
    /// </summary>
    public void UpdateTime(float percent)
    {
        // Update the timer display
        amountLeft.fillAmount = percent;
    }
}
