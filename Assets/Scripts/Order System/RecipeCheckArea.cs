using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeCheckArea : MonoBehaviour
{
    [SerializeField] OrderManager orderManager = null;
    public List<Ingredient> ingredients;
	public float checkRadius = 0.1f;
	public Vector3 checkOffset;
	public float checkUnderDistance = 0.05f;
	[SerializeField]
	AudioSource audioSource = null;
	[SerializeField]
	AudioClip correct = null;
	[SerializeField]
	AudioClip incorrect = null;
	[SerializeField]
	float cooldown = 1.0f;

	[SerializeField] private GameObject foodSpawnLocation = null;

	float cooldownTimer = 0.0f;
	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();
	}
	private void OnTriggerEnter(Collider other)
    {
		if (!enabled)
			return;
		if (!other.attachedRigidbody)
			return;


        else if(cooldownTimer <= 0 && other.tag == "Check")
        {
			CheckIngredients();
		}
    }

	public void CheckIngredients()
	{
		cooldownTimer = cooldown;
		Vector3 test = transform.TransformDirection(checkOffset);
		var cols = Physics.OverlapSphere(transform.position + test, checkRadius);
		for (int i = 0; i < cols.Length; i++)
		{
			var rb = cols[i].attachedRigidbody;
			if (rb)
			{
				if (cols[i].transform.position.y + checkUnderDistance < (transform.position + test).y)
					continue;

				var ingredient = rb.GetComponent<Ingredient>();
				if (ingredient && !ingredients.Contains(ingredient))
				{
					ingredients.Add(ingredient);
				}
			}
		}

		if (orderManager.CheckRecipe(ingredients, foodSpawnLocation.transform))
		{
			for (int i = 0; i < ingredients.Count; i++)
			{
				Destroy(ingredients[i].gameObject);
			}


			if (audioSource && correct && ingredients.Count > 0)
				audioSource.PlayOneShot(correct);
		}
		else if (audioSource && incorrect && ingredients.Count > 0)
			audioSource.PlayOneShot(incorrect);
		
		ingredients.Clear();
	}

	private void Update()
	{
		cooldownTimer -= Time.deltaTime;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = new Color(1, 0, 0, 0.5f);
		Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
		Gizmos.DrawSphere(checkOffset, checkRadius);
	}
}
