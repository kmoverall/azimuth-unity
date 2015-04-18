using UnityEngine;
using System.Collections;

//This script needs to be attached to every SpriteRenderer in the scene, to ensure proper draw order
[ExecuteInEditMode]
[RequireComponent(typeof(SpriteRenderer))]
public class DrawOrderCorrection : MonoBehaviour {
	SpriteRenderer render;
	void Start() {
		render = gameObject.GetComponent<SpriteRenderer>();
	}

	// Update is called once per frame
	void Update () {
		render.sortingOrder = (int)(gameObject.transform.position.z * -1000);
	}
}
