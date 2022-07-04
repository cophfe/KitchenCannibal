using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class TorsoShake : MonoBehaviour
{
	public bool EnableOrganShake { get; set; } = false;
	[SerializeField]
	float shakeSensitivity = 1.0f;
	[SerializeField]
	ParticleSystem system;
	[SerializeField]
	float organVelocityMagnitude = 1.0f;
	[SerializeField]
	float shakeMagnitude = 2.0f;
	[SerializeField]
	float organsPerShake = 0.25f;
	[SerializeField]
	Transform organParent;
	[SerializeField]
	float shakeTimeout = 0.3f;
	[SerializeField]
	Collider[] torsoColliders; //i am so sorry for making like 8 different collider lists
	[SerializeField]
	Renderer torsoRenderer;
	[SerializeField]
	UnityEvent onTorsoShake;

	float shakeTimeoutTime = 0.0f;
	float organCount = 0.0f;
	Rigidbody rb;
	Vector3 lastVelocity = Vector3.zero;
	List<Collider> organColliders;


	Vector3 lowPassValue = Vector3.zero;
	//https://stackoverflow.com/questions/31389598/how-can-i-detect-a-shake-motion-on-a-mobile-device-using-unity3d-c-sharp
	//framerate dependant solution but whatever
	
	private void OnEnable()
	{
		rb = GetComponent<Rigidbody>();
		organColliders = new List<Collider>();
	}

	private void FixedUpdate()
	{
		DetectShake();

		for (int i = 0; i < organColliders.Count; i++)
		{
			if (!organColliders[i].bounds.Intersects(torsoRenderer.bounds))
			{
				foreach (var torsoCol in torsoColliders)
					Physics.IgnoreCollision(organColliders[i], torsoCol, false);

				organColliders.RemoveAt(i);
				i--;
			}
		}



	}

	void DetectShake()
	{
		shakeTimeoutTime -= Time.deltaTime;

		Vector3 acceleration = (rb.velocity - lastVelocity) * Time.deltaTime;
		lowPassValue = Vector3.Lerp(lowPassValue, acceleration, Time.deltaTime * shakeSensitivity);
		Vector3 deltaAccel = acceleration - lowPassValue;
		lastVelocity = rb.velocity;

		if (EnableOrganShake && shakeTimeoutTime <= 0 && deltaAccel.sqrMagnitude > shakeMagnitude)
		{
			shakeTimeoutTime = shakeTimeout;
			organCount += organsPerShake;

			if (organCount > 1.0f)
			{
				organCount -= 1.0f;
				ShakeOrgan();
			}
		}
	}

	void ShakeOrgan()
	{
		if (organParent.childCount > 0)
		{
			GameObject organ = organParent.GetChild(0).gameObject;

			organ.SetActive(true);
			organ.transform.parent = null;
			var rigidBody = organ.GetComponent<Rigidbody>();
			if (rigidBody)
			{
				Vector3 velocity = rb.velocity;
				velocity += Random.onUnitSphere * organVelocityMagnitude;
				rigidBody.velocity = velocity;
				system.transform.forward = rigidBody.velocity.normalized;
				system.Play();
				var cols = rigidBody.GetComponentsInChildren<Collider>();

				foreach (var col in cols)
				{
					foreach (var torsoCol in torsoColliders)
						Physics.IgnoreCollision(col, torsoCol);
				}
				organColliders.AddRange(cols);
			}

			onTorsoShake?.Invoke();
		}
	}
}
