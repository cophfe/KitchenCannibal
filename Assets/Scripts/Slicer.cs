using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using UnityEngine.Rendering;
using Unity.Mathematics;
using static Unity.Mathematics.math;
//thanku internet for finding memcpy for me
using Unity.Collections.LowLevel.Unsafe;

public class Slicer : MonoBehaviour
{
	public GameObject target;
	public bool slice = false;
	//Slice up is transform.up
	//plane direction right is transform.right

	private void OnValidate()
	{
		if (Application.isPlaying && slice)
		{
			Slice(target, transform.up, transform.position);
			slice = false;
		}
	}

	private void Start()
	{
		if (slice)
			Slice(target, transform.up, transform.position);	
		slice = false;
	}

	static Vector3 GetIntersection(ref Vector3 p0, ref Vector3 p1, float d0, float d1)
	{
		float p = d0 / (d0 - d1);
		return p0 + p * (p1 - p0);
	}

	//should work with meshes that are made 100% out of triangles and are convex 
	//also need to use 16 bit indexing and needs to have normals and tangents and uvs
	static bool Slice(GameObject target, Vector3 worldPlaneNormal, Vector3 worldOrigin)
	{
		
		//check if object is slicable
		//also check if its been sliced before
		//1 submesh will be used as the slice surface submesh on it, add slice surface triangles to that submesh

		if (target == null)
			return false;
		MeshFilter mf1 = target.GetComponent<MeshFilter>();
		if (!mf1)
			return false;

		//check if it is too small to be cut (requires volume calculation, probably just using bounding box for performance)
		
		List<Vector3>[] verts = null;
		List<Vector3> intersections = null;
		Vector3 planeNormal = mf1.transform.InverseTransformDirection(worldPlaneNormal);
		float origin = Vector3.Dot(mf1.transform.InverseTransformPoint(worldOrigin), planeNormal);

		using (Mesh.MeshDataArray targetMeshData = Mesh.AcquireReadOnlyMeshData(mf1.sharedMesh))
		{
			//get vertices from location 0
			var vertices = targetMeshData[0].GetVertexData<float3>(0);
			//get uvs from location 3
			var uvs = targetMeshData[0].GetVertexData<float2>(3);
			//also get tris
			var tris = targetMeshData[0].GetIndexData<ushort>();
		
		
			verts = new List<Vector3>[2]
			{
				new List<Vector3>(vertices.Length),
				new List<Vector3>(vertices.Length)
			};
			intersections = new List<Vector3>(vertices.Length / 2);

			float[] distanceToPlane = new float[3];
			bool[] positiveDistance = new bool[3];

			for (int i = 0; i < tris.Length; i += 3)
			{
				Vector3 p0 = vertices[tris[i]];
				Vector3 p1 = vertices[tris[i + 1]];
				Vector3 p2 = vertices[tris[i + 2]];
				distanceToPlane[0] = Vector3.Dot(p0, planeNormal) - origin;
				distanceToPlane[1] = Vector3.Dot(p1, planeNormal) - origin;
				distanceToPlane[2] = Vector3.Dot(p2, planeNormal) - origin;
				positiveDistance[0] = distanceToPlane[0] > 0;
				positiveDistance[1] = distanceToPlane[1] > 0;
				positiveDistance[2] = distanceToPlane[2] > 0;

				if (positiveDistance[0] == positiveDistance[1] && positiveDistance[0] == positiveDistance[2])
				{
					//curse you c#
					int sideIndex = positiveDistance[0] ? 0 : 1;

					verts[sideIndex].Add(p0);
					verts[sideIndex].Add(p1);
					verts[sideIndex].Add(p2);
				}
				else
				{
					//in this case the plane goes through the triangle
					//now need to create 3 triangles with og points and the 2 points of intersection with the plane

					Vector3 int1 = Vector3.zero;
					Vector3 int2 = Vector3.zero;
					int sideIndex0 = 0;
					int sideIndex1 = 0;

					if (positiveDistance[0] == positiveDistance[1])
					{
						int1 = GetIntersection(ref p1, ref p2, distanceToPlane[1], distanceToPlane[2]);
						int2 = GetIntersection(ref p2, ref p0, distanceToPlane[2], distanceToPlane[0]);

						sideIndex0 = positiveDistance[0] ? 0 : 1;
						sideIndex1 = 1 - sideIndex0;

						//do side 1
						verts[sideIndex0].Add(p0);
						verts[sideIndex0].Add(p1);
						verts[sideIndex0].Add(int1);

						verts[sideIndex0].Add(p0);
						verts[sideIndex0].Add(int1);
						verts[sideIndex0].Add(int2);
					
						//do side 2
						verts[sideIndex1].Add(int1);
						verts[sideIndex1].Add(p2);
						verts[sideIndex1].Add(int2);
					}
					else if (positiveDistance[0] == positiveDistance[2])
					{
						int1 = GetIntersection(ref p0, ref p1, distanceToPlane[0], distanceToPlane[1]);
						int2 = GetIntersection(ref p1, ref p2, distanceToPlane[1], distanceToPlane[2]);

						sideIndex0 = positiveDistance[0] ? 0 : 1;
						sideIndex1 = 1 - sideIndex0;

						//do side 1
						verts[sideIndex0].Add(p0);
						verts[sideIndex0].Add(int1);
						verts[sideIndex0].Add(p2);

						verts[sideIndex0].Add(int1);
						verts[sideIndex0].Add(int2);
						verts[sideIndex0].Add(p2);

						//do side 2
						verts[sideIndex1].Add(int1);
						verts[sideIndex1].Add(p1);
						verts[sideIndex1].Add(int2);
					}
					else
					{
						int1 = GetIntersection(ref p0, ref p1, distanceToPlane[0], distanceToPlane[1]);
						int2 = GetIntersection(ref p0, ref p2, distanceToPlane[0], distanceToPlane[2]);

						sideIndex0 = positiveDistance[2] ? 0 : 1;
						sideIndex1 = 1 - sideIndex0;
					
						//do side 1
						verts[sideIndex1].Add(p0);
						verts[sideIndex1].Add(int1);
						verts[sideIndex1].Add(int2);

						//do side 2
						verts[sideIndex0].Add(int1);
						verts[sideIndex0].Add(p1);
						verts[sideIndex0].Add(p2);

						verts[sideIndex0].Add(int1);
						verts[sideIndex0].Add(p2);
						verts[sideIndex0].Add(int2);

					}

					//add slice plane
					intersections.Add(int2);
					intersections.Add(int1);
					//if (!first)
					//{
						//first = true;
						//firstPoint = int1;
					//}
					//else
					//{
						//Vector3 n = Vector3.Cross(int2 - int1, firstPoint - int1);
						//float d = Vector3.Dot(n, planeNormal);
						//if (d > 0)
						//{
							//intersections.Add(int2);
							//intersections.Add(int1);

									//verts[0].Add(int2);
							//verts[0].Add(int1);
							//verts[0].Add(firstPoint);

							//verts[1].Add(int1);
							//verts[1].Add(int2);
							//verts[1].Add(firstPoint);
						//}
						//else
						//{
							//verts[0].Add(int1);
							//verts[0].Add(int2);
							//verts[0].Add(firstPoint);

							//verts[1].Add(int2);
							//verts[1].Add(int1);
							//verts[1].Add(firstPoint);
						//}
					//}

				}
			}
		}

		//sort intersections using
		////Vector3 n = Vector3.Cross(int2 - int1, firstPoint - int1);
		//float d = Vector3.Dot(n, planeNormal);
		//if (d > 0) swap int2 with int1

		//https://catlikecoding.com/unity/tutorials/procedural-meshes/creating-a-mesh/
		//allocate meshes using the better mesh api, for performance
		//there is a strong possibility I'm making this slower with this method

		Mesh[] meshes = null; 
		using (Mesh.MeshDataArray meshDatas = Mesh.AllocateWritableMeshData(2))
		{
			meshes = new Mesh[2];
			for (int i = 0; i < 2; i++)
			{
				Mesh.MeshData data = meshDatas[i];
				var vertexAttributes = new NativeArray<VertexAttributeDescriptor>(4, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
				//pos
				vertexAttributes[0] = new VertexAttributeDescriptor(dimension: 3);
				//normal
				vertexAttributes[1] = new VertexAttributeDescriptor(
				VertexAttribute.Normal, dimension: 3, stream: 1
				);
				//tangent
				vertexAttributes[2] = new VertexAttributeDescriptor(
					VertexAttribute.Tangent, dimension: 4, stream: 2
				);
				//uv
				vertexAttributes[3] = new VertexAttributeDescriptor(
					VertexAttribute.TexCoord0, dimension: 2, stream: 3
				);

				//SET VERTICES
				int submesh1VertCount = verts[0].Count;
				int submesh2VertCount = intersections.Count - 2; // +, if sliced before, previous meshe's sliced vertices
				int count = submesh1VertCount + submesh2VertCount;
				data.SetVertexBufferParams(count, vertexAttributes);
				var rawVertices = data.GetVertexData<float3>();
				CopyFromList(ref rawVertices, 0, verts[i], verts[i].Count);
				CopyFromList(ref rawVertices, verts[i].Count, intersections, intersections.Count - 2);

				//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
				//SET NORMALS
				//SET TANGENTS
				//SET UVS
				vertexAttributes.Dispose();

				int intersectionTriangles = (intersections.Count - 2) / 2;
				data.SetIndexBufferParams(verts[0].Count + intersectionTriangles * 3, IndexFormat.UInt16);
				NativeArray<ushort> triangles = data.GetIndexData<ushort>();

				for (ushort vertIndex = 0; vertIndex < submesh1VertCount; i++)
					triangles[vertIndex] = vertIndex;
				
				int addition = i;
				int otherAddition = 1 - i;
				for (ushort triIndex = 0; triIndex < count; triIndex++)
				{
					int vertIndex = 3 * triIndex;
					triangles[vertIndex] = (ushort)(submesh1VertCount + triIndex * 2 + addition);
					triangles[vertIndex] = (ushort)(submesh1VertCount + triIndex * 2 + otherAddition);
					triangles[vertIndex] = (ushort)(count - 1);
				}

				//set submeshes
				data.subMeshCount = 2;
				data.SetSubMesh(0, new SubMeshDescriptor(0, submesh1VertCount), MeshUpdateFlags.DontRecalculateBounds);
				data.SetSubMesh(0, new SubMeshDescriptor(submesh1VertCount, submesh2VertCount));
			}

			Mesh.ApplyAndDisposeWritableMeshData(meshDatas, meshes);
		}

		//could use an object pool for sliced pieces, setup with components already
		//create object 1
		{
			mf1.sharedMesh = meshes[0];
		}
		//create object 2
		GameObject gO2;
		{
			gO2 = Instantiate(target);
			var mF2 = gO2.GetComponent<MeshFilter>();
			mF2.sharedMesh = meshes[0];
		}

		//setup collider
		var mC1 = target.GetComponent<MeshCollider>();
		if (mC1)
		{
			var mC2 = gO2.GetComponent<MeshCollider>();

			mC1.sharedMesh = meshes[0];
			mC2.sharedMesh = meshes[1];
		}
		return true;

		
	}

	static unsafe void CopyFromList(ref NativeArray<float3> nativeArray, int offset, List<Vector3> list, int count)
	{
		//two memcpys and an allocation vs 1 memcpy, 1 allocation & a iterate through the list
		var array = list.ToArray();
		fixed (void* vertexBufferPointer = array)
		{
			UnsafeUtility.MemCpy((float3*)NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(nativeArray) + offset,
			vertexBufferPointer, count * (long)UnsafeUtility.SizeOf<float3>());
		}
	}

	static unsafe void CopyFromArray(ref NativeArray<float3> nativeArray, int offset, Vector3[] array, int count)
	{
		fixed (void* vertexBufferPointer = array)
		{
			UnsafeUtility.MemCpy((float3*)NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(nativeArray) + offset,
			vertexBufferPointer, count * (long)UnsafeUtility.SizeOf<float3>());
		}
	}
}
