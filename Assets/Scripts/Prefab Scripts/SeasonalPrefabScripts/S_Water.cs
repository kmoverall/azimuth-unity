using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
public class S_Water : SeasonalObject {

	private List<SeasonEmitter> o_IntersectingNodes;
	private BoxCollider2D waterCollider;

    //Ice prefab must only be a BoxCollider2D with a scale of 1x1 units
	public BoxCollider2D icePrefab;
    private List<BoxCollider2D> iceBoxes;

	// Use this for initialization
	protected override void Initialize () {
        waterCollider = gameObject.GetComponent<BoxCollider2D>();
        iceBoxes = new List<BoxCollider2D>();
	}
	
	// Update is called once per frame
	void Update () {
        Bounds tmpBounds = new Bounds();
		for(int i = 0; i < o_IntersectingNodes.Count; i++) {
            tmpBounds = FindIntersectionBounds(o_IntersectingNodes[i]);
            //Check if the intersection actually exists
            if (tmpBounds.min != Vector3.zero && tmpBounds.max != Vector3.zero) {
                iceBoxes[i].transform.position = new Vector3((tmpBounds.max.x + tmpBounds.min.x) / 2, (tmpBounds.max.y + tmpBounds.min.y) / 2, 0);
                iceBoxes[i].transform.localScale = new Vector3(tmpBounds.max.x - tmpBounds.min.x, tmpBounds.max.y - tmpBounds.min.y, 1);
            } else {
                o_IntersectingNodes.RemoveAt(i);
                iceBoxes.RemoveAt(i);
                //Ensure that index remains at the appropriate amount
                i--;
            }
        }
	}

	public override void ReactToNode(SeasonEmitter node) {
		o_IntersectingNodes.Add(node);
        iceBoxes.Add(Instantiate<BoxCollider2D>(icePrefab));
	}

	protected override void DoTransition(Season toSeason) {}

    private Bounds FindIntersectionBounds(SeasonEmitter node) {
        Bounds results = new Bounds();
        List<Vector3> corners = new List<Vector3>();
        List<Vector3> candidatePoints = new List<Vector3>();
        List<Vector3> intersects;
        List<Vector3> selectedPoints = new List<Vector3>();

		Vector3 resultMin;
		Vector3 resultMax;

        //Add corners of collider
        corners.Add(new Vector3(waterCollider.bounds.max.x, waterCollider.bounds.max.y, 0));
        corners.Add(new Vector3(waterCollider.bounds.max.x, waterCollider.bounds.min.y, 0));
        corners.Add(new Vector3(waterCollider.bounds.min.x, waterCollider.bounds.max.y, 0));
        corners.Add(new Vector3(waterCollider.bounds.min.x, waterCollider.bounds.min.y, 0));

        //Add quadrants of node
        candidatePoints.Add(new Vector3(node.transform.position.x, node.transform.position.y + node.radius, 0));
        candidatePoints.Add(new Vector3(node.transform.position.x, node.transform.position.y - node.radius, 0));
        candidatePoints.Add(new Vector3(node.transform.position.x + node.radius, node.transform.position.y, 0));
        candidatePoints.Add(new Vector3(node.transform.position.x - node.radius, node.transform.position.y, 0));

        //Add intersects with top of collider
        intersects = FindCircleLineIntersect(node.transform.position, node.radius, waterCollider.bounds.max.x, waterCollider.bounds.min.x, waterCollider.bounds.max.y, waterCollider.bounds.max.y);
        foreach (Vector3 p in intersects) {
            candidatePoints.Add(p);
        }

        //Add intersects with bottom of collider
        intersects = FindCircleLineIntersect(node.transform.position, node.radius, waterCollider.bounds.max.x, waterCollider.bounds.min.x, waterCollider.bounds.min.y, waterCollider.bounds.min.y);
        foreach (Vector3 p in intersects) {
            candidatePoints.Add(p);
        }

        //Add intersects with left side of collider
        intersects = FindCircleLineIntersect(node.transform.position, node.radius, waterCollider.bounds.min.x, waterCollider.bounds.min.x, waterCollider.bounds.max.y, waterCollider.bounds.min.y);
        foreach (Vector3 p in intersects) {
            candidatePoints.Add(p);
        }

        //Add intersects with right side of collider
        intersects = FindCircleLineIntersect(node.transform.position, node.radius, waterCollider.bounds.max.x, waterCollider.bounds.max.x, waterCollider.bounds.max.y, waterCollider.bounds.min.y);
        foreach (Vector3 p in intersects) {
            candidatePoints.Add(p);
        }

		//Adds non-corner points that are within the bounds of the water
		foreach (Vector3 p in candidatePoints) {
			if (waterCollider.OverlapPoint(p)) {
				selectedPoints.Add(p);
			}
		}
		//Adds corner points that are within the node
		foreach (Vector3 corner in corners) {
			if (Vector3.Distance(corner, node.transform.position) <= node.radius) {
				selectedPoints.Add(corner);
			}
		}

		//Find the bounding rectangle of the selectedPoints
		resultMax = selectedPoints[0];
		resultMin = selectedPoints[0];
		for (int i = 1; i < selectedPoints.Count; i++) {
			//Set Minimum
			if (selectedPoints[i].x < resultMin.x) {
				resultMin.x = selectedPoints[i].x;
			}
			if (selectedPoints[i].y < resultMin.y) {
				resultMin.y = selectedPoints[i].y;
			}

			//Set Maximum
			if (selectedPoints[i].x > resultMax.x) {
				resultMax.x = selectedPoints[i].x;
			}
			if (selectedPoints[i].y > resultMax.y) {
				resultMax.y = selectedPoints[i].y;
			}
		}

        if (resultMin != resultMax) {
            results.SetMinMax(resultMin, resultMax);
        }
        else {
            results.SetMinMax(Vector3.zero, Vector3.zero);
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
