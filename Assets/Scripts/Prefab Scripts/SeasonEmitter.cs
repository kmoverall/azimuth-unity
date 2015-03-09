using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SphereCollider))]
public class SeasonEmitter : MonoBehaviour {
    SphereCollider cEffectBounds;

	public Season nodeSeason = Season.Summer;
	bool isActive = true;

	// Use this for initialization
	void Start () {
        cEffectBounds = gameObject.GetComponent<SphereCollider>();
		cEffectBounds.isTrigger = true;
    }
	
	// Update is called once per frame
	void Update () {
		Shader.SetGlobalVector("_NODE_POSITION", gameObject.transform.position);
		Shader.SetGlobalFloat("_NODE_SIZE", cEffectBounds.radius);
		Shader.SetGlobalInt("_NODE_SEASON", (int)nodeSeason);
	}
}
