using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class OutOfBounds : MonoBehaviour
{
	[SerializeField]
	new Camera camera;
	[SerializeField]
	float fadeSpeed = 1.0f;

	[SerializeField]
	LayerMask boundsMask;

	bool isOutOfBounds = false;

	ColorAdjustments colorAdjuster;
	float fadeAmount = 0;
	private void Awake()
	{		
		var volume = camera.GetComponentInChildren<Volume>();
		if ((volume && volume.profile.TryGet<ColorAdjustments>(out colorAdjuster)));
		{
		}

		if (colorAdjuster == null)
			enabled = false;

	}

	private void OnTriggerStay(Collider other)
	{
		isOutOfBounds |= (1 << other.gameObject.layer & boundsMask.value) != 0;
	}


	private void FixedUpdate()
	{
		UpdateFadeEffect();
		isOutOfBounds = false;
	}
	void UpdateFadeEffect()
	{
		fadeAmount = Mathf.MoveTowards(fadeAmount, isOutOfBounds ? 1 : 0, fadeSpeed * Time.deltaTime);
		colorAdjuster.active = fadeAmount > 0;

		float t = 1 - fadeAmount;
		colorAdjuster.colorFilter.value = Color.white * t;
	}
}
