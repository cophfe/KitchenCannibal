using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cookable : Ingredient
{
	[SerializeField]
	bool processedThroughCook = true;

	[SerializeField]
	Color burntColour;
	[SerializeField]
	Color cookedColour;
	[SerializeField]
	new MeshRenderer renderer;
	[SerializeField, Min(0)]
	int materialIndex = 0;

	float cookSpeed = 0;

	Material material;

	Color startColour;
	Color endColour;
	bool cooked = false;

	public bool PreviousCookState { get; private set; }
	public bool Cooking { get; set; }

	private void Awake()
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
					enabled = false;

				cooked = true;
			}
		}

		Cooking = false;

	}
}
