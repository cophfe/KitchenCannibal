using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeCut : MonoBehaviour
{
	[SerializeField]
	Material defaultSliceMaterial;
	[SerializeField]
	float sliceSpeed = 0.5f;
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
		if (!enabled || !doOnCollision)
			return;

		if (colliders != null)
		{
			var col = collision.GetContact(0).thisCollider;
			for (int i = 0; i < colliders.Length; i++)
			{
				if (col == colliders[i])
				{
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
		if (sliceable != null || sliceTimer > 0)
		{
			float velocity = Vector3.Dot(rb.velocity, transform.TransformDirection(localSpeedDirection.normalized));
			Debug.Log(velocity);
			if (velocity > sliceSpeed)
			{
				List<Sliceable> sliceables = slicer.Slice(sliceable, transform.TransformDirection(localSlicePlaneDirection).normalized, sliceOrigin ? sliceOrigin.position : transform.position);

				if (sliceable != null)
					sliceTimer = sliceTimeout;

				if (colliders != null && ignoreCollisionAfterHit && sliceables != null && sliceables.Count > 0)
				{
					for (int i = 0; i < sliceables.Count; i++)
					{
						var col = sliceables[i].GetComponent<MeshCollider>();

						if (col)
						{
							for (int j = 0; j < colliders.Length; j++)
							{
								Physics.IgnoreCollision(col, colliders[j]);
							}
							justSliced.Add(new ColData() { timeElapsed = 0, collider = col }) ;
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
					keep |= cols[i] == col.collider;
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
		Debug.DrawRay(sliceOrigin.position, transform.TransformDirection(localSlicePlaneDirection.normalized), Color.red, Time.deltaTime, false);
		Debug.DrawRay(sliceOrigin.position, transform.TransformDirection(localSpeedDirection.normalized), Color.blue, Time.deltaTime, false);

		Gizmos.color = new Color(1, 0, 0, 0.5f);
		Quaternion rot = Quaternion.LookRotation(transform.forward, transform.TransformDirection(localSlicePlaneDirection));
		Gizmos.matrix = Matrix4x4.TRS(sliceOrigin.position, rot, Vector3.one);
		Gizmos.DrawCube(ignoreBoxOffset, ignoreBoxExtents);

	}

}
