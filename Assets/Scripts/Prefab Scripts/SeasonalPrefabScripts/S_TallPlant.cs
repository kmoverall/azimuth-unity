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

	protected override void DoTransition(Season toSeason) {
		if (toSeason != Season.Spring) {
			GetComponent<Collider2D>().isTrigger = true;
			gameObject.layer = 10;
		}

		if (toSeason == Season.Spring) {
			GetComponent<Collider2D>().isTrigger = false;
			gameObject.layer = 0;
		}
	}
}
