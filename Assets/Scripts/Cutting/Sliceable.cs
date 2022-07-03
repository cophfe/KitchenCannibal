using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliceable : MonoBehaviour
{
	[field: SerializeField]
	public Material SliceMaterial { get; private set; } = null;

	public bool CanBeSliced { get; set; } = true;
	public int TimesSliced { get; set; } = 0;

	public Sliceable ParentSliceable { get; set; }
	public Transform SliceHolder { get; set; }

	public Rigidbody AttachedRigidbody { get; set; }
	public bool Held { get; set; } = false;

	public Ingredient Ingredient { get; set; }
	private void Start()
	{
		AttachedRigidbody = GetComponent<Rigidbody>();
		Ingredient = GetComponent<Ingredient>();
	}

}
