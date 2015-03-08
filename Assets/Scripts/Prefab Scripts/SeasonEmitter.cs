using UnityEngine;
using System.Collections;

public class SeasonEmitter : MonoBehaviour {
    SphereCollider cEffectBounds;

	// Use this for initialization
	void Start () {
        cEffectBounds = gameObject.GetComponent<SphereCollider>();
		cEffectBounds.isTrigger = true;
    }
	
	// Update is called once per frame
	void Update () {
		Shader.SetGlobalVector("_NODE_POSITION", gameObject.transform.position);
		Shader.SetGlobalFloat("_NODE_SIZE", cEffectBounds.radius);
		Shader.SetGlobalInt("_GLOBAL_SEASON", 0);
		Shader.SetGlobalInt("_NODE_SEASON", 1);
	}
}
