using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
//The standard season object animates when the seasons change.
//Though this sounds simple, Mecanim allows for a lot of variety with just this
public class S_StandardSeasonObject : SeasonalObject {
	Animator animator;

	new protected void Start() {
		animator = GetComponent<Animator>();
		base.Start();
	}

	public override void ReactToNode(SeasonEmitter node) { }

	protected override void DoTransition(Season toSeason) {
		animator.SetInteger("SeasonChange", (int)toSeason);
	}

	protected override void Initialize() {
		animator.SetInteger("SeasonChange", (int)currentSeason);
	}
}
