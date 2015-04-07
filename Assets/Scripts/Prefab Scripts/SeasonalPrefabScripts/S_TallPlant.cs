using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class S_TallPlant : SeasonalObject {
	Animator animator;

	new protected void Start() {
		animator = GetComponent<Animator>();
		base.Start();
	}

	protected override void DoTransition(Season toSeason) {
		Debug.Log((int)toSeason);
		animator.SetInteger("SeasonChange", (int)toSeason);
		animator.SetTrigger("SeasonTrigger");
	}

	protected override void Initialize() {
		animator.SetInteger("SeasonChange", (int)currentSeason);
		animator.SetTrigger("SeasonTrigger");
	}
}
