using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
public class S_Water : SeasonalObject {
    
	private BoxCollider2D waterCollider;

    //Ice prefab must only be a BoxCollider2D with a scale of 1x1 units
	public BoxCollider2D icePrefab;
    private Dictionary<SeasonEmitter, BoxCollider2D> iceBlocks = new Dictionary<SeasonEmitter, BoxCollider2D>();

	// Use this for initialization
	protected override void Initialize () {
        waterCollider = gameObject.GetComponent<BoxCollider2D>();
	}
	
	// Update is called once per frame
	void Update () {
		if (GameManager.NodeSeason == Season.Winter) {
			foreach (SeasonEmitter node in iceBlocks.Keys) {
				if (node.IsActive) {
					Bounds tmpBounds;
					tmpBounds = FindIntersectionBounds(node);

					iceBlocks[node].gameObject.layer = 0;
					iceBlocks[node].transform.position = new Vector3((tmpBounds.max.x + tmpBounds.min.x) / 2, (tmpBounds.max.y + tmpBounds.min.y) / 2, 0);
					iceBlocks[node].transform.localScale = new Vector3(tmpBounds.max.x - tmpBounds.min.x, tmpBounds.max.y - tmpBounds.min.y, 1);
				}
				else {
					iceBlocks[node].gameObject.layer = 10;
				}
			}
		}
		else {
			foreach (BoxCollider2D box in iceBlocks.Values) {
				box.gameObject.layer = 10;
			}
		}
	}

	public override void ReactToNode(SeasonEmitter node) {
        if (!iceBlocks.ContainsKey(node)) {
            iceBlocks.Add(node, Instantiate<BoxCollider2D>(icePrefab));
        }
    }

	protected override void DoTransition(Season toSeason) { }

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
        intersects = FindCircleLineIntersect(node.transform.position, node.radius, waterCollider.bounds.max.y, false);
        foreach (Vector3 p in intersects) {
            candidatePoints.Add(p);
        }

        //Add intersects with bottom of collider
		intersects = FindCircleLineIntersect(node.transform.position, node.radius, waterCollider.bounds.min.y, false);
        foreach (Vector3 p in intersects) {
            candidatePoints.Add(p);
        }

        //Add intersects with left side of collider
		intersects = FindCircleLineIntersect(node.transform.position, node.radius, waterCollider.bounds.min.x, true);
		foreach (Vector3 p in intersects) {
            candidatePoints.Add(p);
        }

        //Add intersects with right side of collider
		intersects = FindCircleLineIntersect(node.transform.position, node.radius, waterCollider.bounds.max.x, true);
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
		if (selectedPoints.Count >= 1) {
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
		}
		else {
			results.SetMinMax(Vector3.zero, Vector3.zero);
		}

		return results;
    }

	//Finds the intersection points of a circle with center and radius and a line at either x = lineCoord or y = lineCoord, depending on isCheckingX
	private List<Vector3> FindCircleLineIntersect(Vector3 center, float radius, float lineCoord, bool isCheckingX) {
		List<Vector3> results = new List<Vector3>();
		float determinant;
		if (isCheckingX) {
			determinant = radius * radius - (lineCoord - center.x) * (lineCoord - center.x);

			//Circle does intersect line
			if (determinant >= 0) {
				results.Add(new Vector3(lineCoord, center.y + Mathf.Sqrt(determinant)));

				//Circle intersects line in multiple points
				if (determinant > 0) {
					results.Add(new Vector3(lineCoord, center.y - Mathf.Sqrt(determinant)));
				}
			} 
		}
		else {
			determinant = radius * radius - (lineCoord - center.y) * (lineCoord - center.y);

			//Circle does intersect line
			if (determinant >= 0) {
				results.Add(new Vector3(center.x + Mathf.Sqrt(determinant), lineCoord));

				//Circle intersects line in multiple points
				if (determinant > 0) {
					results.Add(new Vector3(center.x - Mathf.Sqrt(determinant), lineCoord));
				}
			} 
		}

		return results;
	}
}
