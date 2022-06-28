using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawPivotPoints : MonoBehaviour
{
	[SerializeField]
	bool drawPivots = true;
	[SerializeField]
	float radius = 0.003f;

	private void OnDrawGizmos()
	{
		if (drawPivots)
		{
			Gizmos.color = Color.blue;
			DrawPivot(transform);
		}
	}

	void DrawPivot(Transform t)
	{
		Gizmos.matrix = Matrix4x4.TRS(t.position, t.rotation, Vector3.one);
		Gizmos.DrawCube(Vector3.right * -(radius/2), new Vector3(radius , radius / 10, radius / 10) );
		Gizmos.DrawSphere(Vector3.zero, radius / 7);

		foreach(Transform child in t)
		{
			DrawPivot(child);
		}
	}	
}
