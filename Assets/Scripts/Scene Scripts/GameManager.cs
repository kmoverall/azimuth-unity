using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviour {
	
	private static GameManager _instance;
	#region Singleton Boilerplate
	public static GameManager instance {
		get {
			if (_instance == null) {
				_instance = GameObject.FindObjectOfType<GameManager>();
				if (_instance == null) {
					Debug.LogError("An instance of GameManager is needed in the scene, but there is none");
				}
			}
			return _instance;
		}
	}
	#endregion

	public Season globalSeason = Season.Spring;
	public Season nodeSeason = Season.Summer;
	#region Managing and accessing the seasons

	public static Season GlobalSeason {
		get {
			return instance.globalSeason;
		}
		set {
			instance.globalSeason = value;
		}
	}

	public static Season NodeSeason {
		get {
			return instance.nodeSeason;
		}
		set {
			instance.nodeSeason = value;
		}
	}
	#endregion

	List<SeasonEmitter> sceneNodes;
	const int kMaxNodes = 4;
	#region Node Functions

	//SeasonEmitters register themselves in the sceneNodes list in their Start() function, so all nodes in the scene are tracked
	//Returns the index of the node registered
	public static int RegisterNode(SeasonEmitter node) {
		if (instance.sceneNodes.Count < kMaxNodes) {
			instance.sceneNodes.Add(node);
			return instance.sceneNodes.Count - 1;
		}
		else {
			//Cleans up nodes that are not being used by the scene
			Debug.LogWarning("Too many nodes in scene! (Max = " + kMaxNodes + ")");
			GameObject.Destroy(node.gameObject);
			return -1;
		}
	}

	//Finds node closest to a given transform
	public static SeasonEmitter GetNearestNode(Vector3 check) {
		if (instance.sceneNodes.Count > 0) {

			float minDistance = Vector3.Distance(check, instance.sceneNodes[0].transform.position);
			int minIndex = 0;

			for (int i = 0; i < instance.sceneNodes.Count; i++) {
				float tmpDistance = Vector3.Distance(check, instance.sceneNodes[i].transform.position);
				if (tmpDistance < minDistance) {
					minDistance = tmpDistance;
					minIndex = i;
				}
			}

			return instance.sceneNodes[minIndex];
		} 
		else {
			return null;
		}
	}

	//Check what season a given transform is in
	public static Season GetSeasonAtPoint(Vector3 check) {

		for (int i = 0; i < instance.sceneNodes.Count; i++) {
			float distance = Vector3.Distance(check, instance.sceneNodes[i].transform.position);

			if (distance < instance.sceneNodes[i].radius) {
				return instance.nodeSeason;
			}
		}

		return instance.globalSeason;
	}
	#endregion

	GameState currentGameState = GameState.Playing;
	public static GameState State {
		get {
			return instance.currentGameState;
		}
	}

	void Awake() {
		instance.sceneNodes = new List<SeasonEmitter>();
	}

	// Update is called once per frame
	void Update () {
		Shader.SetGlobalInt("_GLOBAL_SEASON", (int)instance.globalSeason);
		Shader.SetGlobalInt("_NODE_SEASON", (int)instance.nodeSeason);

		if (instance.sceneNodes.Count < GameManager.kMaxNodes) {
			for (int i = instance.sceneNodes.Count; i < GameManager.kMaxNodes; i++ ) {
				Shader.SetGlobalInt("_NODE" + i + "_ACTIVE", 0);
			}
		}
	}
}
