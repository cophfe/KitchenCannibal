using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CollisionTriggerEvent : MonoBehaviour
{
	public System.Action<Collider> OnTriggerEnterCallback { get; set; }
	public System.Action<Collider> OnTriggerStayCallback { get; set; }
	public System.Action<Collider> OnTriggerExitCallback { get; set; }

	private void OnTriggerEnter(Collider other)
	{
		OnTriggerEnterCallback?.Invoke(other);
	}

	private void OnTriggerStay(Collider other)
	{
		OnTriggerStayCallback?.Invoke(other);
	}

	private void OnTriggerExit(Collider other)
	{
		OnTriggerExitCallback?.Invoke(other);
	}
}
