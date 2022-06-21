using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class DeviceSimDisabler : MonoBehaviour
{
	[SerializeField]
	LocomotionProvider provider;

	private void OnEnable()
	{
#if !UNITY_EDITOR
        gameObject.SetActive(false);
#endif

		provider.beginLocomotion += OnLocomotion;
		provider.endLocomotion += OnLocomotion;
	}

	private void OnDisable()
	{
		provider.beginLocomotion -= OnLocomotion;
		provider.endLocomotion -= OnLocomotion;

	}

	void OnLocomotion(LocomotionSystem system)
	{
		Debug.Log(":0000");
	}

}
