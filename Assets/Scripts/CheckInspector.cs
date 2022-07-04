using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckInspector : MonoBehaviour
{
	[SerializeField]
	Vector3 extents;
	[SerializeField]
	LayerMask layerMask;

	public bool AreAllInBox()
	{
		var colliders = Physics.OverlapBox(transform.position, extents, transform.rotation, layerMask.value, QueryTriggerInteraction.Ignore);
		List<Ingredient> ingredients = null;// GameManager.Instance.ingredients

		foreach(var ingredient in ingredients)
		{
			if (ingredient.ingredientType == IngredientType.humanMeat)
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

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
		Gizmos.DrawCube(Vector3.zero, extents);
	}
}
