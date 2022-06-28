using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliceable : MonoBehaviour
{
	[field: SerializeField]
	public Material SliceMaterial { get; private set; } = null;

	public bool CanBeSliced { get; set; } = true;
	public int TimesSliced { get; set; } = 0;
	
}
