using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[field: SerializeField]
	public HandManager LeftHand { get; private set; }

	[field: SerializeField]
	public HandManager RightHand{ get; private set; }
}
