using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//[RequireComponent (typeof(LineRenderer))]
[RequireComponent (typeof(BoxCollider2D))]
public class TEST_DynamicWater : MonoBehaviour {
	List<float> xPositions;
	List<float> yPositions;
	List<float> velocities;
	List<float> accelerations;
	LineRenderer c_Body;
	List<GameObject> meshObjects;
	List<Mesh> meshes;
	List<GameObject> colliders;

	const float k_SpringConstant = 0.02f;
	const float k_Damping = 0.04f;
	const float k_Spread = 0.05f;
	const float k_Z = -1.0f;

	float baseHeight;
	float left;
	float bottom;

	public Material surfaceMaterial;
	public Material bodyMaterial;
	public GameObject waterMesh;

	// Use this for initialization
	void Start () {
		yPositions = new List<float>();
		xPositions = new List<float>();
		velocities = new List<float>();
		accelerations = new List<float>();
		meshObjects = new List<GameObject>();
		meshes = new List<Mesh>();
		colliders = new List<GameObject>();

		baseHeight = 0;
		left = 0;
		bottom = 0;

		left++;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		//Apply forces to water
		for (int i = 0; i < xPositions.Count; i++) {
			float force = k_SpringConstant * (yPositions[i] - baseHeight) + velocities[i] * k_Damping;
			accelerations[i] = -1*force;
			yPositions[i] += velocities[i];
			velocities[i] += accelerations[i];
			c_Body.SetPosition(i, new Vector3(xPositions[i], yPositions[i], k_Z));
		}

		//Create wave propogation
		float[] leftDeltas = new float[xPositions.Count];
		float[] rightDeltas = new float[xPositions.Count];

		for (int j = 0; j < 8; j++) {
			for (int i = 0; i < xPositions.Count; i++) {
				if (i > 0) {
					leftDeltas[i] = k_Spread * (yPositions[i] - yPositions[i - 1]);
					velocities[i - 1] += leftDeltas[i];
				}
				if (i < xPositions.Count - 1) {
					rightDeltas[i] = k_Spread * (yPositions[i] - yPositions[i + 1]);
					velocities[i + 1] += rightDeltas[i];
				}
			}
		}

		//Set positions of surface points to wave propogated positions
		for (int i = 0; i < xPositions.Count; i++) {
			if (i > 0) {
				yPositions[i - 1] += leftDeltas[i];
			}
			if (i < xPositions.Count - 1) {
				yPositions[i + 1] += rightDeltas[i];
			}
		}

		UpdateMeshes();
	}

	public void GenerateWater(float left, float width, float top, float bottom) {
		int edgeCount = Mathf.RoundToInt(width) * 5;
		int nodeCount = edgeCount + 1;

		//Draw Surface Line
		c_Body = gameObject.AddComponent<LineRenderer>();
		c_Body.material = surfaceMaterial;
		c_Body.material.renderQueue = 1000;
		c_Body.SetVertexCount(nodeCount);
		c_Body.SetWidth(0.1f, 0.1f);

		for (int i = 0; i < nodeCount; i++) {
			yPositions[i] = top;
			xPositions[i] = left + width * i / edgeCount;
			accelerations[i] = 0;
			velocities[i] = 0;
			c_Body.SetPosition(i, new Vector3(xPositions[i], yPositions[i], k_Z));
		}

		//Set up vertices of Mesh
		for (int i = 0; i < edgeCount; i++) {
			meshes[i] = new Mesh();
			Vector3[] vertices = new Vector3[4];
			vertices[0] = new Vector3(xPositions[i], yPositions[i], k_Z);
			vertices[1] = new Vector3(xPositions[i+1], yPositions[i+1], k_Z);
			vertices[2] = new Vector3(xPositions[i], bottom, k_Z);
			vertices[3] = new Vector3(xPositions[i + 1], bottom, k_Z);

			Vector2[] uvs = new Vector2[4];
			uvs[0] = new Vector2(0, 1);
			uvs[1] = new Vector2(1, 1);
			uvs[2] = new Vector2(0, 0);
			uvs[3] = new Vector2(1, 0);

			int[] tris = new int[6] { 0, 1, 3, 3, 2, 0 };
			meshes[i].vertices = vertices;
			meshes[i].uv = uvs;
			meshes[i].triangles = tris;

			//Create mesh objects
			meshObjects[i] = Instantiate(waterMesh, Vector3.zero, Quaternion.identity) as GameObject;
			meshObjects[i].GetComponent<MeshFilter>().mesh = meshes[i];
			meshObjects[i].transform.parent = transform;

			//Create Colliders
			colliders[i] = new GameObject();
			colliders[i].name = "Trigger";
			colliders[i].AddComponent<BoxCollider2D>();
			colliders[i].transform.parent = transform;
			colliders[i].transform.position = new Vector3(left + width * (i + 0.5f) / edgeCount, top - 0.5f, 0);
			colliders[i].transform.localScale = new Vector3(width / edgeCount, 1, 1);
			colliders[i].GetComponent<BoxCollider2D>().isTrigger = true;
		}
	}

	void UpdateMeshes() {
		for (int i = 0; i < meshes.Count; i++) {
			Vector3[] vertices = new Vector3[4];
			vertices[0] = new Vector3(xPositions[i], yPositions[i], k_Z);
			vertices[1] = new Vector3(xPositions[i + 1], yPositions[i], k_Z);
			vertices[2] = new Vector3(xPositions[i], bottom, k_Z);
			vertices[3] = new Vector3(xPositions[i + 1], bottom, k_Z);

			meshes[i].vertices = vertices;
		}
	}
}
