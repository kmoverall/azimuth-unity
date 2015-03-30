using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CircleCollider2D))]
public class SeasonEmitter : MonoBehaviour {

	//TODO: cEffectBounds needs to be phased out once the custom editor is put in place. It doesn't really do anything
    CircleCollider2D cEffectBounds;
	Collider2D[] eEffectedSeasonalObjects = new Collider2D[30];

	//TODO: Create a custom inspector for season emitters
	[HideInInspector] bool isActive = true;
	public bool IsActive { get {return isActive; } }
	[HideInInspector] public float radius;
	const float kMaxDepth = -1;
	const float kMinDepth = 1;

	//Node index is used to assign the correct shader values
	int nodeIndex;

	public void Activate() {
		isActive = true;
		cEffectBounds.enabled = true;

		foreach (Collider2D o in eEffectedSeasonalObjects) {
			if (o != null && o.gameObject.GetComponent<SeasonalObject>() != null) {
				o.gameObject.GetComponent<SeasonalObject>().Transition(GameManager.NodeSeason);
			}
		}
	}

	public void Deactivate() {
		isActive = false;
		cEffectBounds.enabled = false;

		foreach (Collider2D o in eEffectedSeasonalObjects) {
			if (o != null && o.gameObject.GetComponent<SeasonalObject>() != null) {
				o.gameObject.GetComponent<SeasonalObject>().Transition(GameManager.GlobalSeason);
			}
		}
	}

	// Use this for initialization
	void Start () {
		cEffectBounds = gameObject.GetComponent<CircleCollider2D>();
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

		//Tells seaonsal objects to transition if needed

		if (Physics2D.OverlapCircleNonAlloc(transform.position, radius, eEffectedSeasonalObjects) == eEffectedSeasonalObjects.Length) {
			Debug.LogWarning("Node intersecting maximum number of objects (" + eEffectedSeasonalObjects.Length + ")!");
		}

		if (isActive) {
			foreach (Collider2D o in eEffectedSeasonalObjects) {
				if (o != null && o.gameObject.GetComponent<SeasonalObject>() != null) {
					o.gameObject.GetComponent<SeasonalObject>().Transition(GameManager.NodeSeason);
				}
			}
		}
	}
}
