using UnityEngine;
using System.Collections;

public class SeasonEmitter : MonoBehaviour {
	Collider2D[] eEffectedSeasonalObjects = new Collider2D[30];
	int numEffectedSeasonalObjects = 0;

	bool isActive = true;
	public bool IsActive { get {return isActive; } }
	public float radius;
	const float k_MaxDepth = -1;
	const float k_MinDepth = 1;

	//Node index is used to assign the correct shader values
	int nodeIndex;

	public void Activate() {
		isActive = true;

		for (int i = 0; i < numEffectedSeasonalObjects; i++) {
			if (eEffectedSeasonalObjects[i].gameObject.GetComponent<SeasonalObject>() != null) {
				eEffectedSeasonalObjects[i].gameObject.GetComponent<SeasonalObject>().Transition(GameManager.NodeSeason);
			}
		}
	}

	public void Deactivate() {
		isActive = false;

		for (int i = 0; i < numEffectedSeasonalObjects; i++) {
			if (eEffectedSeasonalObjects[i].gameObject.GetComponent<SeasonalObject>() != null) {
				eEffectedSeasonalObjects[i].gameObject.GetComponent<SeasonalObject>().Transition(GameManager.GlobalSeason);
			}
		}
	}

	// Use this for initialization
	void Start () {
		nodeIndex = GameManager.RegisterNode(this);
    }
	
	// Update is called once per frame
	void Update () {
		Shader.SetGlobalVector("_NODE" + nodeIndex + "_POSITION", gameObject.transform.position);
		Shader.SetGlobalFloat("_NODE" + nodeIndex + "_SIZE", radius);
		if (isActive) {
			Shader.SetGlobalInt("_NODE" + nodeIndex + "_ACTIVE", 1);
		} else {
			Shader.SetGlobalInt("_NODE" + nodeIndex + "_ACTIVE", 0);
		}

		//Tells seasonal objects to transition if needed

		numEffectedSeasonalObjects = Physics2D.OverlapCircleNonAlloc(transform.position, radius, eEffectedSeasonalObjects);

		if (numEffectedSeasonalObjects == eEffectedSeasonalObjects.Length) {
			Debug.LogWarning("Node intersecting maximum number of objects (" + eEffectedSeasonalObjects.Length + ")!");
		}

		for (int i = 0; i < numEffectedSeasonalObjects; i++) {
			if (eEffectedSeasonalObjects[i].gameObject.GetComponent<SeasonalObject>() != null && isActive) {
					eEffectedSeasonalObjects[i].gameObject.GetComponent<SeasonalObject>().Transition(GameManager.NodeSeason);
			}
		}
	}

	void OnDrawGizmos() {
		Gizmos.DrawWireSphere(transform.position, radius);
	}
}
