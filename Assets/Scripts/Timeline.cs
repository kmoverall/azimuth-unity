using UnityEngine;
using System.Collections;

enum TimelineState {
	Playing,
	Reversing,
	Paused
}

public class Timeline : MonoBehaviour {
	AnimationCurve curve;
	float parameter;
	float time = 0;
	TimelineState state = TimelineState.Paused;

	public AnimationCurve Curve {
		set { curve = value; }
	}

	public float Value {
		get { return parameter; }
	}

	void Awake() {
		parameter = 0;
	}

	void Update() {

		switch (state) {
			case (TimelineState.Playing):
				time += Time.deltaTime;
				if (time > curve.keys[curve.length-1].time) {
					time = curve.keys[curve.length-1].time;
					state = TimelineState.Paused;
				}
				break;

			case (TimelineState.Reversing):
				time -= Time.deltaTime;
				if (time < 0) {
					time = 0.0f;
					state = TimelineState.Paused;
				}
				break;
		}

		parameter = curve.Evaluate(time);
	}

	public void Play() { state = TimelineState.Playing; }

	public void Reverse() { state = TimelineState.Reversing; }

	public void Pause() { state = TimelineState.Paused; }

	public void PlayFromStart() { state = TimelineState.Playing; time = 0; }

	public void ReverseFromEnd() { state = TimelineState.Reversing; time = 0; }

	public void SkipToBeginning() { time = 0; }

	public void SkipToEnd() { time = curve.keys[curve.length].time; }

	public void SkipTo(float t) { time = t; }
}
