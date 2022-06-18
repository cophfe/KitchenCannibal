using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR;
using UnityEngine;

public class Hand : MonoBehaviour
{
	private InputDevice rightHand;
	private InputDevice leftHand;

    void Start()
    {
		var inputDevices = new List<InputDevice>();
		InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right, inputDevices);

		if (inputDevices.Count > 0)
			rightHand = inputDevices[0];

	}

    void Update()
    {
        
    }
}
