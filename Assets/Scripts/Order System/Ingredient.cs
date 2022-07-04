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
    public bool hasBoneShards = false;

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
            case IngredientType.Lettuce:
                ingredientType = IngredientType.SlicedLettuce;
                break;

            case IngredientType.SlicedLettuce:
                // Already finished processing
                break;

            case IngredientType.Tomatoe:
                ingredientType = IngredientType.SlicedTomatoe;
                break;

            case IngredientType.SlicedTomatoe:
                // Already finished processing
                break;

            case IngredientType.MincedMeat:
                ingredientType = IngredientType.CookedMeat;
                break;

            case IngredientType.CookedMeat:
                ingredientType = IngredientType.BurntMeat;
                break;

            case IngredientType.BurntMeat:
                // Already finished processing
                break;

            case IngredientType.Bread:
                ingredientType = IngredientType.SlicedBread;
                break;

            case IngredientType.SlicedBread:
                // Already finished processing
                break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.sqrMagnitude > GameManager.Instance.minimumCollisionVelocity)
        {
            switch (ingredientType)
            {
                case IngredientType.Lettuce:
                    audiosource.PlayOneShot(GameManager.Instance.audioManager.GetClip(SoundSources.Vegetable, Random.Range(4, 9)));
                    break;

                case IngredientType.SlicedLettuce:
                    audiosource.PlayOneShot(GameManager.Instance.audioManager.GetClip(SoundSources.Vegetable, Random.Range(4, 9)));
                    break;

                case IngredientType.Tomatoe:
                    audiosource.PlayOneShot(GameManager.Instance.audioManager.GetClip(SoundSources.Vegetable, Random.Range(4, 9)));
                    break;

                case IngredientType.SlicedTomatoe:
                    audiosource.PlayOneShot(GameManager.Instance.audioManager.GetClip(SoundSources.Vegetable, Random.Range(4, 9)));
                    break;

                case IngredientType.MincedMeat:
                    audiosource.PlayOneShot(GameManager.Instance.audioManager.GetClip(SoundSources.Meat, Random.Range(0, 2)));
                    break;

                case IngredientType.CookedMeat:
                    audiosource.PlayOneShot(GameManager.Instance.audioManager.GetClip(SoundSources.Meat, Random.Range(0, 2)));
                    break;

                case IngredientType.BurntMeat:
                    audiosource.PlayOneShot(GameManager.Instance.audioManager.GetClip(SoundSources.Meat, Random.Range(0, 2)));
                    break;

                case IngredientType.Bread:
                    audiosource.PlayOneShot(GameManager.Instance.audioManager.GetClip(SoundSources.Bread, Random.Range(1, 4)));
                    break;

                case IngredientType.SlicedBread:
                    audiosource.PlayOneShot(GameManager.Instance.audioManager.GetClip(SoundSources.Bread, Random.Range(1, 4)));
                    break;
            }
        }
    }

    public void PlayKnifeSound()
    {
        switch (ingredientType)
        {
            case IngredientType.Lettuce:
                audiosource.PlayOneShot(GameManager.Instance.audioManager.GetClip(SoundSources.Vegetable, Random.Range(0, 5)));
                break;

            case IngredientType.SlicedLettuce:
                audiosource.PlayOneShot(GameManager.Instance.audioManager.GetClip(SoundSources.Vegetable, Random.Range(0, 5)));
                break;

            case IngredientType.Tomatoe:
                audiosource.PlayOneShot(GameManager.Instance.audioManager.GetClip(SoundSources.Vegetable, Random.Range(0, 5)));
                break;

            case IngredientType.SlicedTomatoe:
                audiosource.PlayOneShot(GameManager.Instance.audioManager.GetClip(SoundSources.Vegetable, Random.Range(0, 5)));
                break;

            case IngredientType.MincedMeat:
                audiosource.PlayOneShot(GameManager.Instance.audioManager.GetClip(SoundSources.Meat, Random.Range(2, 4)));
                break;

            case IngredientType.CookedMeat:
                audiosource.PlayOneShot(GameManager.Instance.audioManager.GetClip(SoundSources.Meat, Random.Range(2, 4)));
                break;

            case IngredientType.BurntMeat:
                audiosource.PlayOneShot(GameManager.Instance.audioManager.GetClip(SoundSources.Meat, Random.Range(2, 4)));
                break;

            case IngredientType.Bread:
                audiosource.PlayOneShot(GameManager.Instance.audioManager.GetClip(SoundSources.Bread, 0));
                break;

            case IngredientType.SlicedBread:
                audiosource.PlayOneShot(GameManager.Instance.audioManager.GetClip(SoundSources.Bread, 0));
                break;
        }
    }

    public void PlaySizzle(bool isOn)
    {
        if (isOn)
        {
            AudioClip tempClip = null;
            switch (ingredientType)
            {
                case IngredientType.Lettuce:
                    tempClip = GameManager.Instance.audioManager.GetClip(SoundSources.Salad, 0);
                    break;

                case IngredientType.SlicedLettuce:
                    tempClip = GameManager.Instance.audioManager.GetClip(SoundSources.Salad, 0);
                    break;

                case IngredientType.Tomatoe:
                    tempClip = GameManager.Instance.audioManager.GetClip(SoundSources.Salad, 0);
                    break;

                case IngredientType.SlicedTomatoe:
                    tempClip = GameManager.Instance.audioManager.GetClip(SoundSources.Salad, 0);
                    break;

                case IngredientType.MincedMeat:
                    tempClip = GameManager.Instance.audioManager.GetClip(SoundSources.Meat, 4);
                    break;

                case IngredientType.CookedMeat:
                    tempClip = GameManager.Instance.audioManager.GetClip(SoundSources.Meat, 4);
                    break;

                case IngredientType.BurntMeat:
                    tempClip = GameManager.Instance.audioManager.GetClip(SoundSources.Meat, 4);
                    break;

                case IngredientType.Bread:
                    tempClip = GameManager.Instance.audioManager.GetClip(SoundSources.Bread, 4);
                    break;

                case IngredientType.SlicedBread:
                    tempClip = GameManager.Instance.audioManager.GetClip(SoundSources.Bread, 4);
                    break;
            }

            audiosource.clip = tempClip;
            audiosource.loop = true;
            audiosource.Play();
        }
        else
        {
            audiosource.loop = false;
            audiosource.Stop();
        }
    }


    private void OnEnable()
    {
        GameManager.Instance.RegisterIngredient(this);
    }
    private void OnDisable()
    {
        GameManager.Instance.DeRegisterIngredient(this);
    }
}

