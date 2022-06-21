using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerMover : MonoBehaviour
{
	[SerializeField]
	LocomotionProvider provider;

	private void OnEnable()
	{
		provider.beginLocomotion += OnLocomotion;
		provider.endLocomotion += OnLocomotion;
	}

	private void OnDisable()
	{
		
	}

	void OnLocomotion(LocomotionSystem system)
	{
		Debug.Log(":0000");
	}

}
