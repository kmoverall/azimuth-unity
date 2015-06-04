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
			//Determine Maximum X value

        }
	}

	public override void ReactToNode(SeasonEmitter node) {
		o_IntersectingNodes.Add(node);
	}

	protected override void DoTransition(Season toSeason) {}

    private Bounds FindIntersectionBounds(SeasonEmitter node) {
        Bounds results = new Bounds();
        List<Vector3> corners = new List<Vector3>();
        List<Vector3> candidatePoints = new List<Vector3>();
        List<Vector3> intersects;
        List<Vector3> selectedPoints = new List<Vector3>();

        //Add corners of collider
        corners.Add(new Vector3(collider.bounds.max.x, collider.bounds.max.y, 0));
        corners.Add(new Vector3(collider.bounds.max.x, collider.bounds.min.y, 0));
        corners.Add(new Vector3(collider.bounds.min.x, collider.bounds.max.y, 0));
        corners.Add(new Vector3(collider.bounds.min.x, collider.bounds.min.y, 0));

        //Add quadrants of node
        candidatePoints.Add(new Vector3(node.transform.position.x, node.transform.position.y + node.radius, 0));
        candidatePoints.Add(new Vector3(node.transform.position.x, node.transform.position.y - node.radius, 0));
        candidatePoints.Add(new Vector3(node.transform.position.x + node.radius, node.transform.position.y, 0));
        candidatePoints.Add(new Vector3(node.transform.position.x - node.radius, node.transform.position.y, 0));

        //Add intersects with top of collider
        intersects = FindCircleLineIntersect(node.transform.position, node.radius, collider.bounds.max.x, collider.bounds.min.x, collider.bounds.max.y, collider.bounds.max.y);
        foreach (Vector3 p in intersects) {
            candidatePoints.Add(p);
        }

        //Add intersects with bottom of collider
        intersects = FindCircleLineIntersect(node.transform.position, node.radius, collider.bounds.max.x, collider.bounds.min.x, collider.bounds.min.y, collider.bounds.min.y);
        foreach (Vector3 p in intersects) {
            candidatePoints.Add(p);
        }

        //Add intersects with left side of collider
        intersects = FindCircleLineIntersect(node.transform.position, node.radius, collider.bounds.min.x, collider.bounds.min.x, collider.bounds.max.y, collider.bounds.min.y);
        foreach (Vector3 p in intersects) {
            candidatePoints.Add(p);
        }

        //Add intersects with right side of collider
        intersects = FindCircleLineIntersect(node.transform.position, node.radius, collider.bounds.max.x, collider.bounds.max.x, collider.bounds.max.y, collider.bounds.min.y);
        foreach (Vector3 p in intersects) {
            candidatePoints.Add(p);
        }




        return results;
    }

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

		return results;
	}
}
