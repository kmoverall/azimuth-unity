using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class S_TallPlant : SeasonalObject {
	public AnimationCurve growthCurve;
	float animationTime;
	float baseYScale;

	new protected void Start() {
		base.Start();
		baseYScale = transform.localScale.y;
	}

	public override void Transition(Season toSeason) {
		if (currentSeason != toSeason) {
			if (toSeason != Season.Spring) {
				GetComponent<Collider2D>().isTrigger = true;
			}

			if (toSeason == Season.Spring) {
				GetComponent<Collider2D>().isTrigger = false;
			}
		}
	}
}
