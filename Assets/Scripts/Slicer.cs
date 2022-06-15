using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slicer : MonoBehaviour
{
	public GameObject target;
	public bool slice = false;
	//Slice up is transform.up
	//plane direction right is transform.right

	private void OnValidate()
	{
		if (Application.isEditor && slice)
		{
			Slice(target);	
		}
		slice = false;
	}
	
	bool Slice(GameObject target)
	{
		if (target == null)
			return false;
		MeshFilter mr = target.GetComponent<MeshFilter>();
		if (!mr)
			return false;

		Mesh m = mr.mesh;
		int[] tris = m.triangles;
		Vector3[] vertices = m.vertices;

		List<int> tris1 = new List<int>();
		List<Vector3> verts1 = new List<Vector3>();

		List<int> tris2 = new List<int>();
		List<Vector3> verts2 = new List<Vector3>();
		Vector3 planeNormal = transform.right;
		float origin = Vector3.Dot(transform.position, planeNormal);
		for (int i = 0; i < tris.Length; i += 3)
		{
			bool b1 = origin - Vector3.Dot(vertices[tris[i]], planeNormal) < 0;
			bool b2 = origin - Vector3.Dot(vertices[tris[i + 1]], planeNormal) < 0;
			bool b3 = origin - Vector3.Dot(vertices[tris[i + 2]], planeNormal) < 0;

			if (b1 && b2 && b3)
			{
				//note: this is bad
				tris1.Add(vertices.Length);
				tris1.Add(vertices.Length + 1);
				tris1.Add(vertices.Length + 2);

				verts1.Add(vertices[tris[i]]);
				verts1.Add(vertices[tris[i + 1]]);
				verts1.Add(vertices[tris[i + 2]]);
			}
			else if (!b1 && !b2 && !b3)
			{
				tris2.Add(vertices.Length);
				tris2.Add(vertices.Length + 1);
				tris2.Add(vertices.Length + 2);

				verts2.Add(vertices[tris[i]]);
				verts2.Add(vertices[tris[i + 1]]);
				verts2.Add(vertices[tris[i + 2]]);
			}
			else
			{
				//do something wack
			}
		}

		m.vertices = verts1.ToArray();
		m.triangles = tris1.ToArray();
		m.RecalculateNormals();
		mr.mesh = m;

		Mesh m2 = new Mesh();
		m2.vertices = verts2.ToArray();
		m2.triangles = tris2.ToArray();
		m2.RecalculateNormals();
		var clone = Instantiate(target);
		clone.GetComponent<MeshFilter>().mesh = m2;
		return true;
	}
}
