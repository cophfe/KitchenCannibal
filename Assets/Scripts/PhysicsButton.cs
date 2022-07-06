using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsButton : MonoBehaviour
{
	[SerializeField]
	Vector3 acceleration = new Vector3(0, 100, 0);
	[SerializeField]
	new Renderer renderer;
	[SerializeField]
	bool coolDownOnPressDown = true;
	[SerializeField]
	Vector3 pressAxis = Vector3.right;
	[SerializeField]
	float pressDistance = 0.05f;
	[SerializeField]
	float unPressableMass = 1000;
	[SerializeField]
	float pressCooldown = 4;

	[field: SerializeField]
	public UnityEvent OnPressed { get; private set; }
	[field: SerializeField]
	public UnityEvent OnStopPress { get; private set; }
	[field: SerializeField]
	public UnityEvent WhilePressed { get; private set; }
	[field: SerializeField]
	public UnityEvent OnStopCooldown { get; private set; }

	public bool Pressed { get; private set; } = false;
	
	bool onCooldown = false;
	Material material;
	static readonly int emissionColourId = Shader.PropertyToID("_EmissionColor");
	Color emissionColour;
	Color emissionColourCapped;
	bool inversePress;
	
	float coolDownTimer = 0;
	Vector3 startPosition;
	Rigidbody rb;
	float mass;

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
		startPosition = transform.localPosition;
		inversePress = pressDistance < 0;
		mass = rb.mass;
		if (renderer)
		{
			material = renderer.material;
			emissionColour = material.GetColor(emissionColourId);
			emissionColourCapped = new Color(Mathf.Clamp01(emissionColour.r), Mathf.Clamp01(emissionColour.g), Mathf.Clamp01(emissionColour.b));
		}
	}

	private void Update()
	{
		if (onCooldown)
		{
			if (coolDownTimer > 0)
			{
				coolDownTimer -= Time.deltaTime;
				float t = 1.0f - coolDownTimer / pressCooldown;
				material?.SetColor(emissionColourId, emissionColourCapped * ( t * t * t));
			}
			else
			{
				onCooldown = false;
				rb.mass = mass;
				material?.SetColor(emissionColourId, emissionColour);
				OnStopCooldown?.Invoke();
			}
		}
		
	}
	private void FixedUpdate()
	{
		rb.AddForce(transform.TransformDirection(acceleration), ForceMode.Acceleration);

		CheckPressed();
	}

	void CheckPressed()
	{
		float pressDist = Vector3.Dot(transform.localPosition - startPosition, pressAxis);

		Debug.Log("dist: " + pressDist);
		if (inversePress)
			CallEvents(pressDist < pressDistance);
		else
			CallEvents(pressDist > pressDistance);
	}

	void CallEvents(bool isPressed)
	{
		//if is pressed and was previously pressed
		if (isPressed && Pressed)
		{
			WhilePressed?.Invoke();
		}
		//if is pressed and wasn't previously pressed
		else if (isPressed)
		{
			if (!coolDownOnPressDown)
				StartCooldown();
			
			OnPressed?.Invoke();
			WhilePressed?.Invoke();
			Pressed = true;
		}
		//if is previously pressed and is no longer pressed
		else if (Pressed)
		{
			if (coolDownOnPressDown)
				StartCooldown();

			OnStopPress?.Invoke();
			Pressed = false;
		}

	}

	void StartCooldown()
	{
		if (pressCooldown <= 0)
			return;
		coolDownTimer = pressCooldown;
		rb.mass = unPressableMass;
		onCooldown = true;
	}
	private void OnDisable()
	{
		CallEvents(false);
	}

	private void OnDrawGizmosSelected()
	{
		Debug.DrawRay(transform.position, transform.TransformDirection(pressAxis) * pressDistance, Color.cyan, 0, false);


	}
}

