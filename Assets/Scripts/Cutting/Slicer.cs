using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using UnityEngine.Rendering;
using Unity.Mathematics;
using static Unity.Mathematics.math;
//thanku internet for finding memcpy for me
using Unity.Collections.LowLevel.Unsafe;

public class Slicer
{
	public float DisallowCuttingVolume { get; set; }
	public float DoNotCreateVolume { get; set; }
	public Material DefaultSliceMaterial { get; set; }

	public Slicer(float disallowCuttingVolume, float doNotCreateVolume, Material defaultSliceMaterial)
	{
		DisallowCuttingVolume = disallowCuttingVolume;
		DoNotCreateVolume = doNotCreateVolume;
		DefaultSliceMaterial = defaultSliceMaterial;
	}

	static Vector3 GetIntersection(ref Vector3 p0, ref Vector3 p1, float d0, float d1)
	{
		float p = d0 / (d0 - d1);
		return p0 + p * (p1 - p0);
	}

	static (Vector3, Vector2) GetIntersection(ref Vector3 p0, ref Vector3 p1, ref Vector2 uv0, ref Vector2 uv1, float d0, float d1)
	{
		float p = d0 / (d0 - d1);
		return (p0 + p * (p1 - p0), uv0 + p * (uv1 - uv0));
	}

	//should work with meshes that are made 100% out of triangles and are convex 
	//also need to use 16 bit indexing and needs to have uvs
	public List<Sliceable> Slice(Sliceable sliceable, Vector3 worldPlaneNormal, Vector3 worldOrigin)
	{
		//vertex data is organised in one stream, interleaved if it was made using regular mesh api
		//getting vertex data without copying (with MeshData.GetVertexData) depends on the internal mesh data, so I will have to do some copying
		//mesh api uses interleaved all in one stream
		//float3 position(12 bytes)
		//float3 normal(12 bytes)
		//byte4 color32(4 bytes) or float4 color(16 bytes)
		//float2 | float3 | float4 uv(8, 12 or 16 bytes)
		//float2 | float3 | float4 uv2(8, 12 or 16 bytes)
		//float2 | float3 | float4 uv3(8, 12 or 16 bytes)
		//float2 | float3 | float4 uv4(8, 12 or 16 bytes)
		//float4 tangent(16 bytes)
		if (!sliceable)
			return null;

		GameObject target = sliceable.gameObject;
		MeshFilter mf1 = target.GetComponent<MeshFilter>();
		if (!mf1)
			return null;

		//check if object is slicable
		if (!sliceable.CanBeSliced)
			return null;


		Mesh readMesh = mf1.sharedMesh;
		List<Vector3>[] verts = null;
		List<Vector2>[] uvs = null;

		List<Vector3>[] previousSliceVerts = null;

		List<Vector3> intersections = null;
		Vector3 planeNormal = target.transform.InverseTransformDirection(worldPlaneNormal);
		float origin = Vector3.Dot(target.transform.InverseTransformPoint(worldOrigin), planeNormal);
		float[] volume = { 0, 0 };

		using (Mesh.MeshDataArray targetMeshData = Mesh.AcquireReadOnlyMeshData(readMesh))
		{
			//get vertices from location 0
			Mesh.MeshData data = targetMeshData[0];
			
			NativeArray<Vector3> vertexArray = new NativeArray<Vector3>(readMesh.vertexCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			data.GetVertices(vertexArray);
			NativeArray<Vector2> uvArray = new NativeArray<Vector2>(readMesh.vertexCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			data.GetUVs(0, uvArray);

			var submesh = data.GetSubMesh(0);

			//also get tris
			var tris = data.GetIndexData<ushort>();
			verts = new List<Vector3>[2]
			{
				new List<Vector3>(submesh.indexCount),
				new List<Vector3>(submesh.indexCount)
			};
			uvs = new List<Vector2>[2]
			{
				new List<Vector2>(submesh.indexCount),
				new List<Vector2>(submesh.indexCount)
			};
			intersections = new List<Vector3>(submesh.vertexCount / 2);

			CalculateSlices(submesh, verts, uvs, intersections, vertexArray,
				uvArray, tris, planeNormal, origin, volume);

			if (sliceable.SlicedBefore)
			{
				submesh = data.GetSubMesh(1);
				previousSliceVerts = new List<Vector3>[2]
				{
					new List<Vector3>(submesh.indexCount),
					new List<Vector3>(submesh.indexCount)
				};
				
				CalculateSlicesOnSliceSurface(submesh, previousSliceVerts, intersections, vertexArray, tris, planeNormal, origin, volume);
			}
			else
			{
				previousSliceVerts = new List<Vector3>[2]
				{
					new List<Vector3>(0),
					new List<Vector3>(0)
				};
			}
			vertexArray.Dispose();
		}

		if (intersections.Count == 0)
			return null;

		//sort intersections so their triangles are all oriented correctly
		Vector3 universalPoint = intersections[intersections.Count - 1];
		float intersectionVolume = 0;
		for (int i = 0; i < intersections.Count - 1; i += 2)
		{
			Vector3 n = Vector3.Cross(intersections[i + 1] - intersections[i], universalPoint - intersections[i]);
			float d = Vector3.Dot(n, planeNormal);
			if (d > 0)
			{
				var cache = intersections[i];
				intersections[i] = intersections[i + 1];
				intersections[i + 1] = cache;


				intersectionVolume += CalculateTriangleVolume(intersections[i], intersections[i + 1], universalPoint);
				
			}
		}
		volume[0] += intersectionVolume;
		volume[1] -= intersectionVolume;

		//https://catlikecoding.com/unity/tutorials/procedural-meshes/creating-a-mesh/
		//allocate meshes using the better mesh api, for performance
		//there is a strong possibility I'm making this slower with this method

		Mesh[] meshes; 
		{
			Mesh.MeshDataArray meshDatas = Mesh.AllocateWritableMeshData(2);
			
			//create two meshes for the two parts
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
					VertexAttribute.TexCoord0, format: VertexAttributeFormat.Float32, dimension: 2, stream: 3
				);

				//SET VERTICES
				int submesh1VertCount = verts[i].Count;
				//copy over intersection vertices

				int submesh2VertCount = intersections.Count - 1 + previousSliceVerts[i].Count; // +, if sliced before, previous meshe's sliced vertices
				int count = submesh1VertCount + submesh2VertCount;
				data.SetVertexBufferParams(count, vertexAttributes);
				var rawVertices = data.GetVertexData<float3>();
				CopyFromList(ref rawVertices, 0, verts[i], verts[i].Count);
				CopyFromList(ref rawVertices, verts[i].Count, intersections, intersections.Count - 2);
				ushort lastVertexIndex = (ushort)(count - previousSliceVerts[i].Count - 1);
				//last vertex is used over and over again in the triangle array
				rawVertices[lastVertexIndex] = intersections[intersections.Count - 1];

				//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
				//SET NORMALS
				//SET TANGENTS
				//eh just auto calculate em with unity function cuz lazy
				//although i know that all intersection triangles have the same normal and tangents so that would be useful

				//SET UVS
				var rawUVs = data.GetVertexData<float2>(3);
				CopyFromList(ref rawUVs, 0, uvs[i], uvs[i].Count);
				
				//I don't care about uvs for submesh 2 so it dont matter they can be broke
				//MemClear(ref rawUVs, uvs[i].Count, count - uvs[i].Count);
				//MemClear(ref rawUVs, 0, rawUVs.Length);
				vertexAttributes.Dispose();


				int intersectIndicesCount = 3 * (intersections.Count - 2) / 2 + previousSliceVerts[i].Count;
				data.SetIndexBufferParams(verts[i].Count + intersectIndicesCount, IndexFormat.UInt16);
				NativeArray<ushort> triangles = data.GetIndexData<ushort>();

				//set triangles
				//do the first submesh triangles
				for (ushort vertIndex = 0; vertIndex < submesh1VertCount; vertIndex++)
				{
					triangles[vertIndex] = vertIndex;
				}
				
				//then do the second submesh triangles
				//second submesh is the slice surface triangles, and is flipped depending on which side it is on
				//they are all connected to a single vertex, the last vertex
				int addition = i;
				int otherAddition = 1 - i;
				int intersectTriangleCount = (intersections.Count - 2) / 2;

				for (int j = 0; j < intersectTriangleCount; j ++)
				{
					int triIndex = submesh1VertCount + j * 3;
					triangles[triIndex] = (ushort)(submesh1VertCount + 2 * j + addition);
					triangles[triIndex + 1] = (ushort)(submesh1VertCount + 2 * j + otherAddition);
					triangles[triIndex + 2] = lastVertexIndex;
				}
				if (sliceable.SlicedBefore)
				{
					//the second submesh also contains previous created slice surface
					int indicesCount = verts[i].Count + intersectIndicesCount;
					ushort startTriangleIndex = (ushort)(indicesCount - previousSliceVerts[i].Count);

					for (ushort j = startTriangleIndex, k = (ushort)(lastVertexIndex + 1); j < indicesCount; j++, k++)
					{
						triangles[j] = k;
					}

					//also set slice positions
					CopyFromList(ref rawVertices, lastVertexIndex + 1, previousSliceVerts[i], previousSliceVerts[i].Count);

				}
				//set submeshes
				data.subMeshCount = 2;
				data.SetSubMesh(0, new SubMeshDescriptor(0, submesh1VertCount), MeshUpdateFlags.DontRecalculateBounds);
				data.SetSubMesh(1, new SubMeshDescriptor(submesh1VertCount, intersectIndicesCount));

				meshes[i] = new Mesh()
				{ name = "Sliced " + i  };
			}

			Mesh.ApplyAndDisposeWritableMeshData(meshDatas, meshes);
		}

		//set meshfilter
		var renderer = mf1.GetComponent<MeshRenderer>();

		if (renderer && (sliceable.SliceMaterial || DefaultSliceMaterial))
		{
			if (renderer.sharedMaterials.Length < 2)
			{
				Material[] materials = new Material[2];
				materials[0] = renderer.sharedMaterials[0];
				materials[1] = sliceable.SliceMaterial ? sliceable.SliceMaterial : DefaultSliceMaterial;

				renderer.sharedMaterials = materials;
			}
		}

		Vector3 scale = target.transform.lossyScale;
		float scaleVol = scale.x * scale.y * scale.z;
		Debug.Log("Total volume of " + target.name + " before scale: " + (volume[0] + volume[1]));
		volume[0] *= scaleVol;
		volume[1] *= scaleVol;
		Debug.Log("Total volume of " + target.name + " after scale: " + (volume[0] + volume[1]));

		List<Sliceable> list = new List<Sliceable>(2);

		//create objects
		bool targetUsed = false;
		for (int i = 0; i < 2; i++)
		{
			Debug.Log("Slice " + i + " Volume: " + volume[i]);

			if (volume[i] < DoNotCreateVolume)
				continue;

			GameObject slice;
			if (targetUsed)
			{
				slice = GameObject.Instantiate(target, target.transform.parent);
			}
			else
			{
				slice = target;
				targetUsed = true;
			}

			meshes[i].RecalculateBounds();
			var sliceableComponent = slice.GetComponent<Sliceable>();

			if (volume[i] < DisallowCuttingVolume)
			{
				//remove sliceable property
				sliceableComponent.CanBeSliced = false;
			}

			sliceableComponent.SlicedBefore = true;
			
			meshes[i].RecalculateNormals();
			meshes[i].RecalculateTangents();

			var mF = slice.GetComponent<MeshFilter>();
			mF.sharedMesh = meshes[i];
			var mC = slice.GetComponent<MeshCollider>();
			if (mC)
				mC.sharedMesh = meshes[i];

			list.Add(sliceableComponent);
		}

		return list;
	}

	void CalculateSlices(SubMeshDescriptor submeshData, List<Vector3>[] verts, List<Vector2>[] uvs, List<Vector3> intersections,
		NativeArray<Vector3> readVerts, NativeArray<Vector2> readUvs, NativeArray<ushort> readTris
		, Vector3 planeNormal, float origin, float[] volume)
	{
		float[] distanceToPlane = new float[3];
		bool[] positiveDistance = new bool[3];
		int indicesEnd = submeshData.indexStart + submeshData.indexCount;

		for (int i = submeshData.indexStart; i < indicesEnd; i += 3)
		{
			//get all values
			Vector3 p0 = readVerts[readTris[i]];
			Vector3 p1 = readVerts[readTris[i + 1]];
			Vector3 p2 = readVerts[readTris[i + 2]];
			Vector2 uv0 = readUvs[readTris[i]];
			Vector2 uv1 = readUvs[readTris[i + 1]];
			Vector2 uv2 = readUvs[readTris[i + 2]];

			//get the distance to plane for all vertices
			distanceToPlane[0] = Vector3.Dot(p0, planeNormal) - origin;
			distanceToPlane[1] = Vector3.Dot(p1, planeNormal) - origin;
			distanceToPlane[2] = Vector3.Dot(p2, planeNormal) - origin;

			//get what side of the plane these vertices are on
			positiveDistance[0] = distanceToPlane[0] > 0;
			positiveDistance[1] = distanceToPlane[1] > 0;
			positiveDistance[2] = distanceToPlane[2] > 0;

			//if they are all on the same side then add the entire triangle to that side
			if (positiveDistance[0] == positiveDistance[1] && positiveDistance[0] == positiveDistance[2])
			{
				//curse you c#
				int sideIndex = positiveDistance[0] ? 0 : 1;

				verts[sideIndex].Add(p0);
				verts[sideIndex].Add(p1);
				verts[sideIndex].Add(p2);
				uvs[sideIndex].Add(uv0);
				uvs[sideIndex].Add(uv1);
				uvs[sideIndex].Add(uv2);

				//find volume of triangles
				volume[sideIndex] += CalculateTriangleVolume(p0, p1, p2);
			}
			//otherwise they are intersecting
			else
			{
				//in this case the plane goes through the triangle
				//now need to create 3 triangles with og points and the 2 points of intersection with the plane
				Vector3 int1;
				Vector3 int2;
				Vector3 intUV1;
				Vector3 intUV2;
				int sideIndex0;
				int sideIndex1;

				if (positiveDistance[0] == positiveDistance[1])
				{
					(int1, intUV1) = GetIntersection(ref p1, ref p2, ref uv1, ref uv2, distanceToPlane[1], distanceToPlane[2]);
					(int2, intUV2) = GetIntersection(ref p2, ref p0, ref uv2, ref uv0, distanceToPlane[2], distanceToPlane[0]);

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

					//also uvs
					//do side 1
					uvs[sideIndex0].Add(uv0);
					uvs[sideIndex0].Add(uv1);
					uvs[sideIndex0].Add(intUV1);

					uvs[sideIndex0].Add(uv0);
					uvs[sideIndex0].Add(intUV1);
					uvs[sideIndex0].Add(intUV2);

					//do side 2
					uvs[sideIndex1].Add(intUV1);
					uvs[sideIndex1].Add(uv2);
					uvs[sideIndex1].Add(intUV2);
				}
				else if (positiveDistance[0] == positiveDistance[2])
				{
					(int1, intUV1) = GetIntersection(ref p0, ref p1, ref uv0, ref uv1, distanceToPlane[0], distanceToPlane[1]);
					(int2, intUV2) = GetIntersection(ref p1, ref p2, ref uv1, ref uv2, distanceToPlane[1], distanceToPlane[2]);

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

					//also uvs
					//do side 1
					uvs[sideIndex0].Add(uv0);
					uvs[sideIndex0].Add(intUV1);
					uvs[sideIndex0].Add(uv2);

					uvs[sideIndex0].Add(intUV1);
					uvs[sideIndex0].Add(intUV2);
					uvs[sideIndex0].Add(uv2);

					//do side 2
					uvs[sideIndex1].Add(intUV1);
					uvs[sideIndex1].Add(uv1);
					uvs[sideIndex1].Add(intUV2);
				}
				else
				{
					(int1, intUV1) = GetIntersection(ref p0, ref p1, ref uv0, ref uv1, distanceToPlane[0], distanceToPlane[1]);
					(int2, intUV2) = GetIntersection(ref p0, ref p2, ref uv0, ref uv2, distanceToPlane[0], distanceToPlane[2]);

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

					//also uvs
					//do side 1
					uvs[sideIndex1].Add(uv0);
					uvs[sideIndex1].Add(intUV1);
					uvs[sideIndex1].Add(intUV2);

					//do side 2
					uvs[sideIndex0].Add(intUV1);
					uvs[sideIndex0].Add(uv1);
					uvs[sideIndex0].Add(uv2);

					uvs[sideIndex0].Add(intUV1);
					uvs[sideIndex0].Add(uv2);
					uvs[sideIndex0].Add(intUV2);
				}

				//add slice plane
				intersections.Add(int2);
				intersections.Add(int1);

				int c0 = verts[sideIndex0].Count;
				int c1 = verts[sideIndex1].Count;
				//find volume of triangles
				volume[sideIndex0] += CalculateTriangleVolume(verts[sideIndex0][c0 - 3], verts[sideIndex0][c0 - 2], verts[sideIndex0][c0 - 1]);
				volume[sideIndex0] += CalculateTriangleVolume(verts[sideIndex0][c0 - 6], verts[sideIndex0][c0 - 5], verts[sideIndex0][c0 - 4]);
				
				volume[sideIndex1] += CalculateTriangleVolume(verts[sideIndex1][c1 - 3], verts[sideIndex1][c1 - 2], verts[sideIndex1][c1 - 1]);
				
			}
		}
	}

	void CalculateSlicesOnSliceSurface(SubMeshDescriptor submeshData, List<Vector3>[] verts, List<Vector3> intersections,
		NativeArray<Vector3> readVerts, NativeArray<ushort> readTris
		, Vector3 planeNormal, float origin, float[] volume)
	{
		float[] distanceToPlane = new float[3];
		bool[] positiveDistance = new bool[3];
		int indicesEnd = submeshData.indexStart + submeshData.indexCount;

		for (int i = submeshData.indexStart; i < indicesEnd; i += 3)
		{
			//get all values
			Vector3 p0 = readVerts[readTris[i]];
			Vector3 p1 = readVerts[readTris[i + 1]];
			Vector3 p2 = readVerts[readTris[i + 2]];

			//get the distance to plane for all vertices
			distanceToPlane[0] = Vector3.Dot(p0, planeNormal) - origin;
			distanceToPlane[1] = Vector3.Dot(p1, planeNormal) - origin;
			distanceToPlane[2] = Vector3.Dot(p2, planeNormal) - origin;

			//get what side of the plane these vertices are on
			positiveDistance[0] = distanceToPlane[0] > 0;
			positiveDistance[1] = distanceToPlane[1] > 0;
			positiveDistance[2] = distanceToPlane[2] > 0;

			//if they are all on the same side then add the entire triangle to that side
			if (positiveDistance[0] == positiveDistance[1] && positiveDistance[0] == positiveDistance[2])
			{
				//curse you c#
				int sideIndex = positiveDistance[0] ? 0 : 1;

				verts[sideIndex].Add(p0);
				verts[sideIndex].Add(p1);
				verts[sideIndex].Add(p2);

				//find volume of triangles
				volume[sideIndex] += CalculateTriangleVolume(p0, p1, p2);
			}
			//otherwise they are intersecting
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

				intersections.Add(int2);
				intersections.Add(int1);

				int c0 = verts[sideIndex0].Count;
				int c1 = verts[sideIndex1].Count;
				//find volume of triangles
				volume[sideIndex0] += CalculateTriangleVolume(verts[sideIndex0][c0 - 3], verts[sideIndex0][c0 - 2], verts[sideIndex0][c0 - 1]);
				volume[sideIndex0] += CalculateTriangleVolume(verts[sideIndex0][c0 - 6], verts[sideIndex0][c0 - 5], verts[sideIndex0][c0 - 4]);

				volume[sideIndex1] += CalculateTriangleVolume(verts[sideIndex1][c1 - 3], verts[sideIndex1][c1 - 2], verts[sideIndex1][c1 - 1]);
			}
		}
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
	static unsafe void CopyFromList(ref NativeArray<float2> nativeArray, int offset, List<Vector2> list, int count)
	{
		//two memcpys and an allocation vs 1 memcpy, 1 allocation & a iterate through the list
		var array = list.ToArray();
		fixed (void* vertexBufferPointer = array)
		{
			UnsafeUtility.MemCpy((float2*)NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(nativeArray) + offset,
			vertexBufferPointer, count * (long)UnsafeUtility.SizeOf<float2>());
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

	static unsafe void MemClear<T>(ref NativeArray<T> nativeArray, int offset, int count) where T : unmanaged
	{
		UnsafeUtility.MemClear((T*)nativeArray.GetUnsafePtr() + offset, count * (long)UnsafeUtility.SizeOf<T>());
	}

	float CalculateVolume(Mesh mesh)
	{
		Bounds b = mesh.bounds;
		return b.size.x * b.size.y * b.size.z;
	}

	static float CalculateTriangleVolume (Vector3 p0, Vector3 p1, Vector3 p2)
	{

		return Vector3.Dot(Vector3.Cross(p0, p1), p2) / 6.0f;
	}
}
