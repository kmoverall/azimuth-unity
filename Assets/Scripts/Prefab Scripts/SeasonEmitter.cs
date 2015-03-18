using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SphereCollider))]
public class SeasonEmitter : MonoBehaviour {
    SphereCollider cEffectBounds;

	//TODO: Create a custom inspector for season emitters

	[HideInInspector] bool isActive = true;
	[HideInInspector] public float radius;
	int nodeIndex;

	public void Activate() {
		isActive = true;
	}

	public void Deactivate() {
		isActive = false;
	}

	// Use this for initialization
	void Start () {
        cEffectBounds = gameObject.GetComponent<SphereCollider>();
		cEffectBounds.isTrigger = true;
		radius = cEffectBounds.radius;

		nodeIndex = GameManager.RegisterNode(this);
    }
	
	// Update is called once per frame
	void Update () {
		Shader.SetGlobalVector("_NODE" + nodeIndex + "_POSITION", gameObject.transform.position);
		Shader.SetGlobalFloat("_NODE" + nodeIndex + "_SIZE", cEffectBounds.radius);
		if (isActive) {
			Shader.SetGlobalInt("_NODE" + nodeIndex + "_ACTIVE", 1);
		} else {
			Shader.SetGlobalInt("_NODE" + nodeIndex + "_ACTIVE", 0);
		}
		
	}
}
