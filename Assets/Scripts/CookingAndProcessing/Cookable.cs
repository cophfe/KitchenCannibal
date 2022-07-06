using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Cookable : Ingredient
{
	[SerializeField]
	bool processedThroughCook = true;
	[SerializeField]
	float timeToBurn = 5.0f;

	[SerializeField]
	Color burntColour;
	[SerializeField]
	Color cookedColour;
	[SerializeField]
	new MeshRenderer renderer;
	[SerializeField, Min(0)]
	int materialIndex = 0;
	[field: SerializeField]
	public UnityEvent OnStartCooking { get; set; }
	[field: SerializeField]
	public UnityEvent OnCookableCook { get; set; }
	[field: SerializeField]
	public UnityEvent OnCookableBurn { get; set; }

	float cookSpeed = 0;

	Material material;

	Color startColour;
	Color endColour;
	bool cooked = false;

	public bool PreviousCookState { get; private set; }
	public bool Cooking { get; set; }

	protected override void Awake()
	{
		//will take 
		cookSpeed = timeToCook == 0 ? Mathf.Infinity : 1.0f / timeToCook;

		List<Material>  materialList = new List<Material>();
		renderer.GetMaterials(materialList);

		if (materialIndex < materialList.Count)
			material = materialList[materialIndex];
		else
		{
			materialIndex = 0;
			material = materialList[0];
		}

		startColour = material.color;
		endColour = cookedColour;

		base.Awake();
	}

	

	private void FixedUpdate()
	{
		PreviousCookState = Cooking;

		if (Cooking)
		{
			//if cooking, increase processed amount
			processedAmount += Time.deltaTime * cookSpeed;
			
			//update material colour
			material.color = Vector4.Lerp(startColour, endColour, processedAmount);
			
			if (processedAmount > 1)
			{
				processedAmount = 0;
				startColour = endColour;
				endColour = burntColour;
				
				if (processedThroughCook)
					Process();

				//after cooking it can cook again once (therefore becoming burnt)
				if (cooked)
				{
					OnCookableBurn?.Invoke();
					enabled = false;
				}
				else
					OnCookableCook?.Invoke();

				cookSpeed = timeToBurn == 0 ? Mathf.Infinity : 1.0f / timeToBurn;

				cooked = true;
			}
		}

		Cooking = false;

	}
}
