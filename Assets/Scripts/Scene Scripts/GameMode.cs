using UnityEngine;
using System.Collections;

public class GameMode : MonoBehaviour {
	public Season globalSeason = Season.Spring;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Shader.SetGlobalInt("_GLOBAL_SEASON", (int)globalSeason);
	}
}
