using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliceable : MonoBehaviour
{
	[field: SerializeField, HideInInspector]
	public Material SliceMaterial { get; private set; } = null;

	[field: SerializeField, HideInInspector]
	public bool CanBeSliced { get; set; } = true;
	[field: SerializeField, HideInInspector]
	public int TimesSliced { get; set; } = 0;

	[field: SerializeField, HideInInspector]
	public Sliceable ParentSliceable { get; set; }
	[field: SerializeField, HideInInspector]
	public Transform SliceHolder { get; set; }

	[field: SerializeField, HideInInspector]
	public Rigidbody AttachedRigidbody { get; set; }
	public bool Held { get; set; } = false;

	public Ingredient Ingredient { get; set; }
	private void Start()
	{
		AttachedRigidbody = GetComponent<Rigidbody>();
		Ingredient = GetComponent<Ingredient>();
	}

}
