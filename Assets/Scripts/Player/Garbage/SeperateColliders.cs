using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SeperateColliders : MonoBehaviour
{
	public bool seperate = false;

	private void Update()
	{
		if (seperate)
		{
			seperate = false;
			var rbs = GetComponentsInChildren<Rigidbody>();
			
			foreach(Rigidbody rb in rbs)
			{
				var col = rb.GetComponent<CapsuleCollider>();
				if (col != null)
				{
					var childCollider = new GameObject("Joint Collider");
					childCollider.transform.parent = rb.transform;
					childCollider.transform.localPosition = Vector3.zero;
					childCollider.transform.localScale = Vector3.one;
					childCollider.transform.localRotation = Quaternion.identity;

					var capsuleCollider = childCollider.AddComponent<CapsuleCollider>();
					capsuleCollider.radius = col.radius;
					capsuleCollider.height = col.height;
					capsuleCollider.contactOffset = col.contactOffset;
					capsuleCollider.direction = col.direction;
					capsuleCollider.center = col.center;
					
					DestroyImmediate(col);
				}
			}
		}
	}

}
