using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SphereCollider))]
public class SeasonEmitter : MonoBehaviour {
    SphereCollider cEffectBounds;

	//TODO: Create a custom inspector for season emitters

	bool isActive = true;
	[HideInInspector] public float radius;

	// Use this for initialization
	void Start () {
        cEffectBounds = gameObject.GetComponent<SphereCollider>();
		cEffectBounds.isTrigger = true;
		radius = cEffectBounds.radius;

		GameManager.RegisterNode(this);
    }
	
	// Update is called once per frame
	void Update () {
		Shader.SetGlobalVector("_NODE_POSITION", gameObject.transform.position);
		Shader.SetGlobalFloat("_NODE_SIZE", cEffectBounds.radius);
		if (isActive) {
			Shader.SetGlobalInt("_NODE_ACTIVE", 1);
		} else {
			Shader.SetGlobalInt("_NODE_ACTIVE", 0);
		}
		
	}
}
