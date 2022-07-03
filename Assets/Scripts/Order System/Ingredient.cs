using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingredient : MonoBehaviour
{
    [HideInInspector] public float processedAmount = 0.0f;
    public float timeToCook = 5.0f;
    public float ingredientAmount = 0.0f;
    public IngredientType ingredientType;
    private AudioSource audiosource = null;

    private void Awake()
    {
        audiosource = GetComponent<AudioSource>();
        if (audiosource == null)
            Debug.Log("Missing component type 'AudioSource'");
    }

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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Knife")
        {
            switch (ingredientType)
            {
                case IngredientType.empty:
                    break;

                case IngredientType.humanMeat:
                    audiosource.PlayOneShot(GameManager.Instance.audioManager.GetClip(SoundSources.Meat, Random.Range(0, 2)));
                    break;

                case IngredientType.mincedHumanMeat:
                    audiosource.PlayOneShot(GameManager.Instance.audioManager.GetClip(SoundSources.Meat, Random.Range(0, 2)));
                    break;

                case IngredientType.cookedMeat:
                    audiosource.PlayOneShot(GameManager.Instance.audioManager.GetClip(SoundSources.Meat, Random.Range(0, 2)));
                    break;

                case IngredientType.buns:
                    audiosource.PlayOneShot(GameManager.Instance.audioManager.GetClip(SoundSources.Bread, Random.Range(1, 5)));
                    break;

                case IngredientType.lettuce:
                    break;

                case IngredientType.tomatoe:
                    break;
            }
        }
        else
        {
            switch (ingredientType)
            {
                case IngredientType.empty:
                    break;
                case IngredientType.humanMeat:
                    break;
                case IngredientType.mincedHumanMeat:
                    break;
                case IngredientType.cookedMeat:
                    break;
                case IngredientType.buns:
                    audiosource.PlayOneShot(GameManager.Instance.audioManager.GetClip(SoundSources.Bread, 0));
                    break;
                case IngredientType.lettuce:
                    break;
                case IngredientType.tomatoe:
                    break;
            }
        }
    }
}

