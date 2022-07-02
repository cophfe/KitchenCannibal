using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grindable : MonoBehaviour
{
	[field: SerializeField]
	public float MeatValue { get; private set; }
	
	//yes yes, another list of colliders
	[field: SerializeField]
	public Collider[] GrindColliders { get; private set; }

	//colliders of this object that have been recorded
	public int RecordedColliders { get; set; } = 0;

	private void Awake()
	{
		if (GrindColliders == null || GrindColliders.Length == 0)
			GrindColliders = transform.GetComponentsInChildren<Collider>();
	}

}
