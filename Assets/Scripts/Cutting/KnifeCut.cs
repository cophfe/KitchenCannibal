using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeCut : MonoBehaviour
{
	[SerializeField]
	Material defaultSliceMaterial;
	[SerializeField]
	float sliceMinSpeed = 0.5f;
	[SerializeField]
	float cutImpulse = 0.04f;
	[SerializeField]
	Vector3 localSlicePlaneDirection = Vector3.right;
	[SerializeField]
	Vector3 localSpeedDirection = Vector3.down;
	[SerializeField]
	Transform sliceOrigin = null;
	[SerializeField]
	bool doOnTrigger = false;
	[SerializeField]
	bool doOnCollision= true;

	[SerializeField]
	bool ignoreCollisionAfterHit = true;
	[SerializeField]
	float ignoreTime = 0.2f;
	[SerializeField]
	float sliceTimeout = 0.1f;
	[SerializeField]
	float noLongerSliceableVolume = 0.1f;
	[SerializeField]
	float deleteSliceableVolume = 0.01f;
	[SerializeField]
	int maxSliceAmount = 4;
	[SerializeField]
	Collider[] colliders = null;

	[SerializeField]
	Vector3 ignoreBoxExtents = Vector3.one;
	[SerializeField]
	Vector3 ignoreBoxOffset = Vector3.zero;

	Slicer slicer;
	
	Rigidbody rb;
	float sliceTimer = 0;

	List<ColData> justSliced;
	struct ColData
	{
		public Collider collider;
		public float timeElapsed;
	}
	private void Start()
	{
		justSliced = new List<ColData> ();

		slicer = new Slicer(noLongerSliceableVolume, deleteSliceableVolume, defaultSliceMaterial);
		rb = GetComponent<Rigidbody>();
		if (!rb)
			rb = GetComponentInParent<Rigidbody>();

		if (!sliceOrigin)
			sliceOrigin = transform;
	}

#if UNITY_EDITOR
	private void OnValidate()
	{
		if (Application.isPlaying)
			slicer = new Slicer(noLongerSliceableVolume, deleteSliceableVolume, defaultSliceMaterial);
	}
#endif

	private void OnTriggerEnter(Collider other)
	{
		if (!enabled || !doOnTrigger)
			return;

		ConsiderSlicing(other.gameObject);


	}

	private void OnCollisionEnter(Collision collision)
	{
		
	}

	private void OnCollisionStay(Collision collision)
	{
		if (!enabled || !doOnCollision)
			return;

		if (colliders != null)
		{
			var col = collision.GetContact(0).thisCollider;
			for (int i = 0; i < colliders.Length; i++)
			{
				if (col == colliders[i])
				{
					Debug.Log("Cutter: " + col.name);
					ConsiderSlicing(collision.gameObject);
					return;
				}
			}
		}
		else
			ConsiderSlicing(collision.gameObject);
	}

	void ConsiderSlicing(GameObject objectToSlice)
	{
		Sliceable sliceable = objectToSlice.GetComponent<Sliceable>();
		if (sliceable != null && sliceTimer < 0 && sliceable.CanBeSliced && sliceable.TimesSliced < maxSliceAmount)
		{
			float velocity = Vector3.Dot(rb.velocity, transform.TransformDirection(localSpeedDirection.normalized));
			//Debug.Log(velocity);
			if (velocity > sliceMinSpeed)
			{
				Vector3 origin = sliceOrigin ? sliceOrigin.position : transform.position;
				Vector3 sliceDirection  = transform.TransformDirection(localSlicePlaneDirection).normalized;

				List<Sliceable> sliceables = slicer.Slice(sliceable, sliceDirection, origin);

				if (sliceable != null && sliceables.Count > 0)
				{
					sliceTimer = sliceTimeout;

					if (colliders != null && ignoreCollisionAfterHit)
					{
						for (int i = 0; i < sliceables.Count; i++)
						{
							//ignore mesh collider until it moves away
							var col = sliceables[i].GetComponent<MeshCollider>();

							if (col)
							{
								for (int j = 0; j < colliders.Length; j++)
								{
									Physics.IgnoreCollision(col, colliders[j]);
								}
								justSliced.Add(new ColData() { timeElapsed = 0, collider = col });
							}
						}

						//also apply a force tangential to the cut plane
						for (int i = 0; i < sliceables.Count; i++)
						{
							var rb = sliceables[i].GetComponent<Rigidbody>();
							if (rb != null)
							{
								float direction = Mathf.Sign(Vector3.Dot(sliceDirection, rb.worldCenterOfMass) - Vector3.Dot(sliceDirection, origin));
								rb.AddForce(direction * sliceDirection * cutImpulse, ForceMode.VelocityChange);
							}
						}
					}
				}

				

				
			}
		}
	}

	private void FixedUpdate()
	{
		sliceTimer -= Time.deltaTime;

		if (colliders != null)
		{
			for (int i = 0; i < justSliced.Count; i++)
			{
				var col = justSliced[i];

				Quaternion rot = Quaternion.LookRotation(transform.forward, transform.TransformDirection(localSlicePlaneDirection));
				Vector3 offset = Matrix4x4.TRS(sliceOrigin.position, rot, Vector3.one).MultiplyVector(ignoreBoxOffset);
				Collider[] cols = Physics.OverlapBox(sliceOrigin.position + offset, ignoreBoxExtents, rot);
				bool keep = false;
				for (int j = 0; j < cols.Length; j++)
				{
					keep |= cols[j] == col.collider;
				}	

				if (!keep)
				{
					col.timeElapsed += Time.deltaTime;
					
					if (col.timeElapsed > ignoreTime)
					{
						for (int j = 0; j < colliders.Length; j++)
						{
							Physics.IgnoreCollision(col.collider, colliders[j], false);
						}
						justSliced.RemoveAt(i);
						i--;
						continue;
					}
				}
				else
				{ 
					col.timeElapsed = 0;
				}
				justSliced[i] = col;
			}
		}

	}

	private void OnDrawGizmosSelected()
	{
		Vector3 sliceOrigin = this.sliceOrigin ? this.sliceOrigin.position : transform.position;
		Debug.DrawRay(sliceOrigin, transform.TransformDirection(localSlicePlaneDirection.normalized), Color.red, 0, false);
		Debug.DrawRay(sliceOrigin, transform.TransformDirection(localSpeedDirection.normalized), Color.blue, 0, false);

		Gizmos.color = new Color(1, 0, 0, 0.5f);
		Quaternion rot = Quaternion.LookRotation(transform.TransformDirection(localSpeedDirection), transform.TransformDirection(localSlicePlaneDirection));
		Gizmos.matrix = Matrix4x4.TRS(sliceOrigin, rot, Vector3.one);
		Gizmos.DrawCube(ignoreBoxOffset, ignoreBoxExtents);

	}

}
