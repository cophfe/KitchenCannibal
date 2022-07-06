using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotDogCollision : MonoBehaviour
{

	private void OnCollisionEnter(Collision collision)
	{
		Debug.Log(collision.other.gameObject.name);
	}
}
