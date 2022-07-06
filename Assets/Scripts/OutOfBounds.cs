using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class OutOfBounds : MonoBehaviour
{
	[SerializeField]
	new Camera camera;
	[SerializeField]
	float fadeSpeed = 1.0f;
	[field: SerializeField]
	public UnityEvent OnEndOverride { get; set; } 

	[SerializeField]
	LayerMask boundsMask;

	bool isOutOfBounds = false;

	bool overrideFade = false;
	ColorAdjustments colorAdjuster;
	float fadeAmount = 0;
	float fadeTarget;
	private void Awake()
	{		
		var volume = camera.GetComponentInChildren<Volume>();
		if (volume && volume.profile.TryGet<ColorAdjustments>(out colorAdjuster));
		{
			OnEnterFade();
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
		if (!overrideFade)
			fadeTarget = isOutOfBounds ? 1 : 0;
	
		isOutOfBounds = false;
	}

	private void Update()
	{
		UpdateFadeEffect();
	}
	void UpdateFadeEffect()
	{
		fadeAmount = Mathf.MoveTowards(fadeAmount, fadeTarget, fadeSpeed * Time.deltaTime);
		colorAdjuster.active = fadeAmount > 0;
		float t = 1 - fadeAmount;
		colorAdjuster.colorFilter.value = Color.white * t;

		if (overrideFade && fadeAmount == fadeTarget)
		{
			overrideFade = false;
			OnEndOverride?.Invoke();
		}
	}

	public void OnEnterFade()
	{
		overrideFade= true;
		fadeAmount = 1.0f;
		fadeTarget = 0;
	}

	public void OnExitFade()
	{
		overrideFade= true;
		fadeTarget = 1.0f;
	}
}
