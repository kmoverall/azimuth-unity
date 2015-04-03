using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class S_TallPlant : SeasonalObject {
	public AnimationCurve growthCurve;
	public BoxCollider2D plantBlock;
	//public Platform2D plantPlatform;

	Timeline growthTimeline;

	float plantBaseYScale;
	float plantBottom;
	float plantCenter;

	new protected void Start() {
		growthTimeline = gameObject.AddComponent<Timeline>();
		growthTimeline.Curve = growthCurve;
		plantBaseYScale = plantBlock.transform.localScale.y;
		plantBottom = plantBlock.bounds.min.y;
		plantCenter = plantBlock.bounds.center.y;
		base.Start();
	}

	void Update() {
		//Vector3 tmpPlatformPosition = plantPlatform.transform.localPosition;

		Vector3 tmpBlockScale = plantBlock.transform.localScale;
		tmpBlockScale.y = plantBaseYScale * growthTimeline.Value;
		plantBlock.transform.localScale = tmpBlockScale;
		
		//Keeps the sprite positioned as expected while the scale is being changed, making it appear to grow from the bottom up
		Vector3 tmpBlockPosition = plantBlock.transform.position;
		tmpBlockPosition.y = Mathf.Lerp(plantBottom, plantCenter, growthTimeline.Value);
		plantBlock.transform.position = tmpBlockPosition;
	}

	protected override void DoTransition(Season toSeason) {
		if (toSeason != Season.Spring) {
			growthTimeline.Reverse();
		}

		if (toSeason == Season.Spring) {
			growthTimeline.Play();
		}
	}

	protected override void Initialize() {
		if (currentSeason != Season.Spring) {
			growthTimeline.SkipToBeginning();
		}

		if (currentSeason == Season.Spring) {
			growthTimeline.SkipToEnd();
		}
	}
}
