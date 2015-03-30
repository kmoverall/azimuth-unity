using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 tmpPos = transform.position;
		tmpPos.y += 0.1f;
		transform.position = tmpPos;
	}
}
