using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class DeviceSimDisabler : MonoBehaviour
{
	private void OnEnable()
	{
#if !UNITY_EDITOR
        gameObject.SetActive(false);
#endif
	}

	private void Start()
	{
		var inputDevices = new List<InputDevice>();
		InputDevices.GetDevices(inputDevices);
		if (inputDevices.Count > 0)
			gameObject.SetActive(false);
	}

}
