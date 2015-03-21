using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class SeasonalObject : MonoBehaviour {
	//Really used exclusively for clarity and ensuring that transitions occur when already overlapping nodes change seasons
	Season currentSeason;
	//Needed to ensure that the object knows if it is current being effect by a node
	int numberOfIntersectingNodes;

	private void Start() {
		currentSeason = GameManager.GlobalSeason;
	}

	//Checks for node season changing while object is affected
	private void Update() {
		if (numberOfIntersectingNodes > 0 && currentSeason != GameManager.NodeSeason) {
			transition(currentSeason, GameManager.NodeSeason);
			currentSeason = GameManager.NodeSeason;
		}
	}

	//Transitions seasons if a node enters
	private void OnTriggerEnter2D(Collider2D other) {
		SeasonEmitter node = other.gameObject.GetComponent<SeasonEmitter>();

		if (node != null && node.IsActive) {
			numberOfIntersectingNodes++;

			if (currentSeason != GameManager.NodeSeason) {
				transition(currentSeason, GameManager.NodeSeason);
				currentSeason = GameManager.NodeSeason;
			}
		}
	}

	//Transitions seasons if a node leaves
	private void OnTriggerExit2D(Collider2D other) {
		SeasonEmitter node = other.gameObject.GetComponent<SeasonEmitter>();

		if (node != null && node.IsActive) {
			numberOfIntersectingNodes--;

			if (numberOfIntersectingNodes == 0) {
				transition(currentSeason, GameManager.GlobalSeason);
				currentSeason = GameManager.GlobalSeason;
			}
		}
	}

	protected abstract void transition(Season fromSeason, Season toSeason);
}
