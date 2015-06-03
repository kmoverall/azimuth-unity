using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
public class S_Water : SeasonalObject {

	private List<SeasonEmitter> o_IntersectingNodes;
	private BoxCollider2D collider;

	public BoxCollider2D icePrefab;
    public List<Bounds> iceBoxes;

	// Use this for initialization
	protected override void Initialize () {
		collider = gameObject.GetComponent<BoxCollider2D>();
        iceBoxes = new List<Bounds>();
	}
	
	// Update is called once per frame
	void Update () {
		for(int i = 0; i < o_IntersectingNodes.Count; i++) {

        }
	}

	public override void ReactToNode(SeasonEmitter node) {
		o_IntersectingNodes.Add(node);
	}

	protected override void DoTransition(Season toSeason) {}

	private List<Vector3> FindCircleLineIntersect(Vector3 center, float radius, float maxX, float minX, float maxY, float minY) {
		List<Vector3> results = new List<Vector3>();
		float deltaX = maxX - minX;
		float deltaY = maxY - minY;
		float deltaR = Mathf.Sqrt(deltaX * deltaX + deltaY * deltaY);
		float determinant = minX * maxY - maxX * minY;

		float discriminant = radius * radius * deltaR * deltaR - determinant * determinant;
		if (discriminant >= 0) {

			results.Add(new Vector3((determinant * deltaY + Mathf.Sign(deltaY) * deltaX * Mathf.Sqrt(discriminant)) / (deltaR * deltaR),
									(-1 * determinant * deltaY + Mathf.Abs(deltaY) * Mathf.Sqrt(discriminant)) / (deltaR * deltaR),
									0));

			//If circle is not tangent to line
			if (discriminant != 0) {
				results.Add(new Vector3((determinant * deltaY - Mathf.Sign(deltaY) * deltaX * Mathf.Sqrt(discriminant)) / (deltaR * deltaR),
									(-1 * determinant * deltaY - Mathf.Abs(deltaY) * Mathf.Sqrt(discriminant)) / (deltaR * deltaR),
									0));
			}
		}
		else {
			return null;
		}

		return results;
	}
}
