using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliceable : MonoBehaviour
{
	[field: SerializeField]
	public Material SliceMaterial { get; private set; } = null;

	public bool SlicedBefore { get; set; } = false;
	public bool CanBeSliced { get; set; } = true;
}
