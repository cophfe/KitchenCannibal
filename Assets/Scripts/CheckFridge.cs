using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckFridge : MonoBehaviour
{
	[SerializeField]
	Vector3 extents;
	[SerializeField]
	LayerMask layerMask;

	public bool AreAllInBox()
	{
		var colliders = Physics.OverlapBox(transform.position, extents / 2, transform.rotation, layerMask.value, QueryTriggerInteraction.Ignore);
		List<Ingredient> ingredients = GameManager.Instance.activeIngredients;

		foreach(var ingredient in ingredients)
		{
			if (ingredient.ingredientType == IngredientType.RawMeat)
			{
				var rb = ingredient.GetComponent<Rigidbody>();
				bool inBox = false;
				foreach(var col in colliders)
				{
					if (col.attachedRigidbody == rb)
					{
						inBox = true;
						break;
					}
				}

				if (!inBox)
				{
					return false;
				}
			}
		}

		return true;
	}

	public void GetAllIngredientsInBox(List<Ingredient> ingredients)
	{
		ingredients.Clear();
		var colliders = Physics.OverlapBox(transform.position, extents /2 , transform.rotation, layerMask.value, QueryTriggerInteraction.Ignore);
		foreach (var col in colliders)
		{
			if (col.attachedRigidbody)
			{
				var ingredient = col.attachedRigidbody.GetComponent<Ingredient>();
				if (ingredient)
					ingredients.Add(ingredient);
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
		Gizmos.DrawCube(Vector3.zero, extents);
	}
}
